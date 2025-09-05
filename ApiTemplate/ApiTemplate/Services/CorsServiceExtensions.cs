using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Services
{
    public static class CorsPolicies
    {
        public const string Default = "DefaultCors";
    }

    public static class CorsServiceExtensions
    {
        /// <summary>
        /// Registers a config-driven CORS policy (Cors:AllowedOrigins in appsettings).
        /// Falls back to AllowAnyOrigin if no origins configured.
        /// </summary>
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()?
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().TrimEnd('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicies.Default, policy =>
                {
                    if (allowedOrigins is { Length: > 0 })
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });

            return services;
        }
    }
}