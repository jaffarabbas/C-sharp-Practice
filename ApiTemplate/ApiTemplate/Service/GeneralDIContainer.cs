using FluentValidation;
using FluentValidation.AspNetCore;
using DBLayer;
using Repositories;
using ApiTemplate.BackgroundServices;
using ApiTemplate.Shared.Services;
using Shared.Services;
using Repositories.Services;
using Microsoft.AspNetCore.Builder;

namespace ApiTemplate.Services
{
    /// <summary>
    /// Provides extension methods for registering all application dependencies in the DI container.
    /// </summary>
    public static class GeneralDIContainer
    {
        /// <summary>
        /// Adds all application services to the DI container and configures logging.
        /// </summary>
        public static WebApplicationBuilder AddGeneralDIContainer(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Configure Serilog logging early in the pipeline
            builder.AddSerilogLogging();

            // Database (moved to DBLayer extension)
            services.AddDatabase(configuration);

            // Repositories / UoW
            services.AddRepositoryDI();

            // CORS
            services.AddConfiguredCors(configuration);

            // API Versioning
            services.AddApiVersioning(configuration);

            // Swagger
            services.AddSwaggerGeneration();
            // services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // Controllers & Endpoints
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Hosted Services
            services.AddHostedService<BGTest>();

            // Authentication
            services.AddAuthenticationSchemeService(configuration);
            services.AddJWTAuthentication(configuration);

            // SignalR & MemoryCache
            services.AddSignalR();
            services.AddMemoryCache();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<TblUsersDtoValidator>();
            services.AddFluentValidationAutoValidation();

            //ratelimiting
            services.AddRateLimiting(configuration);

            // Email Services
            services.AddEmailServices(configuration);

            // Audit Logging Service
            services.AddHttpContextAccessor();
            services.AddScoped<IAuditLoggingService, AuditLoggingService>();

            return builder;
        }

        /// <summary>
        /// Legacy method for backward compatibility. Use AddGeneralDIContainer(WebApplicationBuilder) instead.
        /// </summary>
        [Obsolete("Use AddGeneralDIContainer(WebApplicationBuilder builder) instead")]
        public static IServiceCollection AddGeneralDIContainer(this IServiceCollection services, IConfiguration configuration)
        {
            // Database (moved to DBLayer extension)
            services.AddDatabase(configuration);

            // Repositories / UoW
            services.AddRepositoryDI();

            // CORS
            services.AddConfiguredCors(configuration);

            // API Versioning
            services.AddApiVersioning(configuration);

            // Swagger
            services.AddSwaggerGeneration();
            // services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // Controllers & Endpoints
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Hosted Services
            services.AddHostedService<BGTest>();

            // Authentication
            services.AddAuthenticationSchemeService(configuration);
            services.AddJWTAuthentication(configuration);

            // SignalR & MemoryCache
            services.AddSignalR();
            services.AddMemoryCache();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<TblUsersDtoValidator>();
            services.AddFluentValidationAutoValidation();

            //ratelimiting
            services.AddRateLimiting(configuration);

            // Email Services
            services.AddEmailServices(configuration);

            // Audit Logging Service
            services.AddHttpContextAccessor();
            services.AddScoped<IAuditLoggingService, AuditLoggingService>();

            return services;
        }
    }
}