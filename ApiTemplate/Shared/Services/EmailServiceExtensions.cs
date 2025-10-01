using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dtos;

namespace Shared.Services
{
    /// <summary>
    /// Extension methods for registering email services.
    /// </summary>
    public static class EmailServiceExtensions
    {
        /// <summary>
        /// Adds email services to the dependency injection container.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure email settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register email service
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}