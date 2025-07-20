using ApiTemplate.Repository;

namespace ApiTemplate.Services
{
    /// <summary>
    /// Provides extension methods for registering application dependencies in the DI container.
    /// </summary>
    public static class DIContainer
    {
        /// <summary>
        /// Registers repository and unit of work services for dependency injection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection instance.</returns>
        public static IServiceCollection AddDIContainer(this IServiceCollection services)
        {
            // Registers the generic repository interface and its implementation for scoped lifetime.
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registers the unit of work interface and its implementation for scoped lifetime.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
