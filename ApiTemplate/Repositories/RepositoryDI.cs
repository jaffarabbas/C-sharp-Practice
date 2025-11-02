using ApiTemplate.Repository;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Repository;
using Repositories.Services;

namespace Repositories
{
    /// <summary>
    /// Provides extension methods for registering repository dependencies in the DI container using Scrutor.
    /// </summary>
    public static class RepositoryDI
    {
        /// <summary>
        /// Registers repository services for dependency injection using Scrutor for automatic assembly scanning.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection instance.</returns>
        public static IServiceCollection AddRepositoryDI(this IServiceCollection services)
        {
            // Register the generic repository interface and its implementation
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ITableOperationService, TableOperationService>(); // used by UnitOfWork
            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #region Service Scanning
            // Authorization Service
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
            #endregion

            return services;
        }
    }
}