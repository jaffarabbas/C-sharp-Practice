using FluentValidation;
using FluentValidation.AspNetCore;
using DBLayer;
using Repositories;
using ApiTemplate.BackgroundServices;

namespace ApiTemplate.Services
{
    /// <summary>
    /// Provides extension methods for registering repository dependencies in the DI container using Scrutor.
    /// </summary>
    public static class GeneralDIContainer
    {
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

            return services;
        }
    }
}