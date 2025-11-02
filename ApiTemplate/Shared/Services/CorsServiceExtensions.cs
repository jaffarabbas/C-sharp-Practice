using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiTemplate.Services
{
    public static class CorsPolicies
    {
        public const string Default = "DefaultCors";
    }

    public static class CorsServiceExtensions
    {
        /// <summary>
        /// SECURITY: Registers a config-driven CORS policy with production safety checks
        /// </summary>
        public static IServiceCollection AddConfiguredCors(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()?
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().TrimEnd('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // SECURITY: Validate CORS configuration
            ValidateCorsConfiguration(allowedOrigins, environment);

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicies.Default, policy =>
                {
                    if (allowedOrigins is { Length: > 0 })
                    {
                        // SECURITY: Validate each origin
                        var validOrigins = ValidateOrigins(allowedOrigins, environment);

                        policy.WithOrigins(validOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        // SECURITY: Only allow permissive CORS in Development
                        if (environment.IsDevelopment())
                        {
                            policy.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        }
                        else
                        {
                            // SECURITY: Fail in production if no origins configured
                            throw new InvalidOperationException(
                                "SECURITY ERROR: CORS allowed origins must be explicitly configured in production. " +
                                "Add allowed origins to Cors:AllowedOrigins in appsettings.json or environment variables.");
                        }
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// SECURITY: Validates CORS configuration
        /// </summary>
        private static void ValidateCorsConfiguration(string[]? allowedOrigins, IWebHostEnvironment environment)
        {
            // In production, CORS origins must be explicitly configured
            if (environment.IsProduction() && (allowedOrigins == null || allowedOrigins.Length == 0))
            {
                throw new InvalidOperationException(
                    "SECURITY ERROR: No CORS origins configured for production environment. " +
                    "Configure Cors:AllowedOrigins in appsettings.Production.json");
            }

            // Validate origin format
            if (allowedOrigins != null)
            {
                foreach (var origin in allowedOrigins)
                {
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        throw new InvalidOperationException(
                            $"SECURITY ERROR: Invalid CORS origin format: '{origin}'. " +
                            "Origins must be absolute URLs (e.g., https://example.com)");
                    }

                    // SECURITY: Warn about insecure origins in production
                    if (environment.IsProduction() && uri.Scheme == "http")
                    {
                        throw new InvalidOperationException(
                            $"SECURITY WARNING: HTTP origin '{origin}' is not secure for production. " +
                            "Use HTTPS origins in production.");
                    }

                    // SECURITY: Warn about localhost in production
                    if (environment.IsProduction() &&
                        (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "0.0.0.0"))
                    {
                        throw new InvalidOperationException(
                            $"SECURITY ERROR: Localhost origin '{origin}' should not be configured in production.");
                    }
                }
            }
        }

        /// <summary>
        /// SECURITY: Validates and filters origins based on environment
        /// </summary>
        private static string[] ValidateOrigins(string[] origins, IWebHostEnvironment environment)
        {
            var validOrigins = new List<string>();

            foreach (var origin in origins)
            {
                if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    // In production, skip insecure origins
                    if (environment.IsProduction() && uri.Scheme == "http")
                    {
                        continue; // Skip HTTP in production
                    }

                    // In production, skip localhost
                    if (environment.IsProduction() &&
                        (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "0.0.0.0"))
                    {
                        continue; // Skip localhost in production
                    }

                    validOrigins.Add(origin);
                }
            }

            if (validOrigins.Count == 0)
            {
                throw new InvalidOperationException(
                    "SECURITY ERROR: No valid CORS origins after validation. " +
                    "Ensure at least one secure origin is configured.");
            }

            return validOrigins.ToArray();
        }
    }
}