using ApiTemplate.Repository;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

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
            
            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Use Scrutor to automatically scan and register all repository implementations
            services.Scan(scan => scan
                .FromAssemblyOf<UnitOfWork>()
                .AddClasses(classes => classes
                    .Where(type => 
                        type.Name.EndsWith("Repository") && 
                        !type.Name.StartsWith("Generic") &&
                        !type.IsGenericTypeDefinition &&
                        type.IsClass && 
                        !type.IsAbstract))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}