using ApiTemplate.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiTemplate.Services
{
    public static class AddJWTService
    {
        /// <summary>
        /// Service for adding middleware for jwt authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var _jwtSettng = configuration.GetSection("JWTSetting");
            var authkey = configuration.GetValue<string>("JWTSetting:securitykey");
            var validIssuer = configuration.GetValue<string>("JWTSetting:ValidIssuer");
            var validAudience = configuration.GetValue<string>("JWTSetting:ValidAudience");

            // SECURITY: Validate JWT configuration at startup
            ValidateJwtConfiguration(authkey, validIssuer, validAudience);

            services.Configure<JWTSetting>(_jwtSettng);
            services.AddAuthentication(item =>
             {
                 item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             }).AddJwtBearer(options =>
             {
                 // SECURITY: Require HTTPS only in production (allow HTTP in development for testing)
                 options.RequireHttpsMetadata = environment.IsProduction();
                 options.SaveToken = true;

                 // IMPORTANT: Prevent default claim name mapping (keeps "userId" as "userId" instead of mapping to a different name)
                 options.MapInboundClaims = false;

                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     // IMPORTANT: Prevent default claim type mapping
                     NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                     RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                     // Validate the signing key
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey)),

                     // SECURITY HARDENING: Validate issuer and audience
                     ValidateIssuer = true,
                     ValidIssuer = validIssuer,

                     ValidateAudience = true,
                     ValidAudience = validAudience,

                     // Validate token expiration
                     ValidateLifetime = true,

                     // Allow some clock skew for expiration (default 5 minutes)
                     ClockSkew = TimeSpan.FromMinutes(5),

                     // SECURITY: Require expiration time
                     RequireExpirationTime = true,

                     // SECURITY: Require signed tokens
                     RequireSignedTokens = true
                 };

                 // SECURITY: Custom event handlers for better security logging
                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         // Log authentication failures (helpful for security monitoring)
                         var logger = context.HttpContext.RequestServices
                             .GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerEvents>>();

                         logger.LogWarning("JWT Authentication failed: {Exception}", context.Exception.Message);

                         return Task.CompletedTask;
                     },
                     OnTokenValidated = context =>
                     {
                         // Optional: Additional custom validation logic here
                         return Task.CompletedTask;
                     },
                     OnChallenge = context =>
                     {
                         // Log authorization challenges
                         var logger = context.HttpContext.RequestServices
                             .GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerEvents>>();

                         logger.LogWarning("JWT Challenge: {Error} - {ErrorDescription}",
                             context.Error, context.ErrorDescription);

                         return Task.CompletedTask;
                     }
                 };
             });
            return services;
        }

        /// <summary>
        /// SECURITY: Validates JWT configuration to ensure secure settings
        /// </summary>
        private static void ValidateJwtConfiguration(string? securityKey, string? issuer, string? audience)
        {
            var errors = new List<string>();

            // Validate security key
            if (string.IsNullOrWhiteSpace(securityKey))
            {
                errors.Add("JWT security key is not configured");
            }
            else if (securityKey.Length < 32)
            {
                errors.Add($"JWT security key is too short ({securityKey.Length} characters). Minimum 32 characters required for security.");
            }
            else if (securityKey.Contains("secret") || securityKey.Contains("key") || securityKey.Contains("password"))
            {
                errors.Add("JWT security key appears to be a placeholder. Use a strong, randomly generated key.");
            }

            // Validate issuer
            if (string.IsNullOrWhiteSpace(issuer))
            {
                errors.Add("JWT ValidIssuer is not configured");
            }

            // Validate audience
            if (string.IsNullOrWhiteSpace(audience))
            {
                errors.Add("JWT ValidAudience is not configured");
            }

            if (errors.Any())
            {
                var errorMessage = "JWT Configuration Errors:\n" + string.Join("\n", errors.Select(e => $"  - {e}"));
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// OBSOLETE: Legacy overload without environment parameter. Use AddJWTAuthentication(services, configuration, environment) instead.
        /// This overload assumes development environment (RequireHttpsMetadata = false).
        /// </summary>
        [Obsolete("Use AddJWTAuthentication(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment) instead")]
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Create a mock environment that is always Development for backward compatibility
            var mockEnvironment = new MockWebHostEnvironment { EnvironmentName = "Development" };
            return AddJWTAuthentication(services, configuration, mockEnvironment);
        }

        /// <summary>
        /// Mock environment for legacy method compatibility
        /// </summary>
        private class MockWebHostEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = "Development";
            public string ApplicationName { get; set; } = string.Empty;
            public string ContentRootPath { get; set; } = string.Empty;
            public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
            public string WebRootPath { get; set; } = string.Empty;
            public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = null!;
        }
    }
}
