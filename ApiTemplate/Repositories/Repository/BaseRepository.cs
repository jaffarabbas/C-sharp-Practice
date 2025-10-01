using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace ApiTemplate.Repository
{
    /// <summary>
    /// Base repository class that provides service resolution capabilities to derived repositories.
    /// This allows specific repositories to resolve only the services they need without polluting UnitOfWork.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public abstract class BaseRepository<T> : GenericRepository<T> where T : class
    {
        protected readonly IServiceProvider ServiceProvider;

        protected BaseRepository(
            TestContext context,
            IDbConnection connection,
            IMemoryCache cache,
            IDbTransaction? transaction,
            IServiceProvider serviceProvider)
            : base(context, connection, cache, transaction)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Resolves a service from the DI container.
        /// Use this method in derived repositories to get the services they need.
        /// </summary>
        /// <typeparam name="TService">The service type to resolve</typeparam>
        /// <returns>The resolved service instance</returns>
        protected TService GetService<TService>() where TService : notnull
        {
            return ServiceProvider.GetRequiredService<TService>();
        }

        /// <summary>
        /// Tries to resolve a service from the DI container.
        /// Returns null if the service is not registered.
        /// </summary>
        /// <typeparam name="TService">The service type to resolve</typeparam>
        /// <returns>The resolved service instance or null</returns>
        protected TService? GetServiceOrDefault<TService>() where TService : class
        {
            return ServiceProvider.GetService<TService>();
        }
    }
}
