using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var apiVersioningBuilder = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(defaultMajor, defaultMinor);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                
                // Support multiple versioning methods
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), // api/v1/test
                    new QueryStringApiVersionReader("version"), // ?version=1.0
                    new HeaderApiVersionReader("X-Version") // Header: X-Version: 1.0
                );
            });

            apiVersioningBuilder.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // e.g. v1, v2
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            return services;
        }
    }
}