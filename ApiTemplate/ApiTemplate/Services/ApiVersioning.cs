using Microsoft.AspNetCore.Mvc;

namespace ApiTemplate.Services
{
    /// <summary>
    /// Provides extension methods for registering API versioning in the DI container.
    /// </summary>
    public static class ApiVersioning
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultMajor = configuration.GetValue<int>("Version:DefaultMajor", 1);
            var defaultMinor = configuration.GetValue<int>("Version:DefaultMinor", 0);

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(defaultMajor, defaultMinor);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // Add API Explorer (for Swagger/Versioning integration)
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // e.g. v1, v1.1, v2
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}