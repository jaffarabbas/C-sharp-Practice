using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Attributes;
using Repositories.Infrastructure;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace Repositories.Factory
{
    /// <summary>
    /// Auto-discovering repository factory using reflection.
    /// Automatically finds and registers all repositories marked with [AutoRegisterRepository].
    /// NO MANUAL REGISTRATION NEEDED!
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IRepositoryContext _context;

        // Cache for repository instances (per UnitOfWork lifetime)
        private readonly ConcurrentDictionary<Type, object> _repositoryCache = new();

        // Cache for repository type mappings (static, shared across all instances)
        private static readonly Lazy<Dictionary<Type, Type>> _repositoryRegistry = new(() => DiscoverRepositories());

        public RepositoryFactory(IRepositoryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a repository instance using auto-discovery.
        /// Repositories are cached per UnitOfWork instance.
        /// </summary>
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var interfaceType = typeof(TRepository);

            // Check cache first
            if (_repositoryCache.TryGetValue(interfaceType, out var cachedRepository))
            {
                return (TRepository)cachedRepository;
            }

            // Find implementation type from registry
            if (!_repositoryRegistry.Value.TryGetValue(interfaceType, out var implementationType))
            {
                throw new InvalidOperationException(
                    $"Repository type '{interfaceType.Name}' is not registered. " +
                    $"Make sure the implementation class is marked with [AutoRegisterRepository(typeof({interfaceType.Name}))]");
            }

            // Create instance using reflection
            var repository = CreateRepositoryInstance(implementationType);

            // Cache and return
            _repositoryCache.TryAdd(interfaceType, repository);
            return (TRepository)repository;
        }

        /// <summary>
        /// Checks if a repository can be created for the specified type.
        /// </summary>
        public bool CanCreate<TRepository>() where TRepository : class
        {
            return _repositoryRegistry.Value.ContainsKey(typeof(TRepository));
        }

        /// <summary>
        /// Discovers all repositories in the assembly marked with [AutoRegisterRepository].
        /// This runs once at application startup.
        /// </summary>
        private static Dictionary<Type, Type> DiscoverRepositories()
        {
            var registry = new Dictionary<Type, Type>();

            // Get the assembly containing repositories
            var assembly = Assembly.GetExecutingAssembly();

            // Find all classes with [AutoRegisterRepository] attribute
            var repositoryTypes = assembly.GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    type.GetCustomAttribute<AutoRegisterRepositoryAttribute>() != null)
                .ToList();

            foreach (var repoType in repositoryTypes)
            {
                var attribute = repoType.GetCustomAttribute<AutoRegisterRepositoryAttribute>();
                if (attribute != null)
                {
                    registry[attribute.InterfaceType] = repoType;
                }
            }

            return registry;
        }

        /// <summary>
        /// Creates a repository instance using reflection.
        /// Automatically determines the correct constructor parameters.
        /// </summary>
        private object CreateRepositoryInstance(Type implementationType)
        {
            // Get the constructor
            var constructor = implementationType.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"Repository type '{implementationType.Name}' does not have a public constructor.");
            }

            // Build parameters based on constructor signature
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                // Map known types from context
                args[i] = paramType.Name switch
                {
                    nameof(TestContext) or "TestContext" => _context.DbContext,
                    nameof(IDbConnection) or "IDbConnection" => _context.DapperConnection,
                    nameof(IDbTransaction) or "IDbTransaction" => _context.DapperTransaction,
                    nameof(IMemoryCache) or "IMemoryCache" => _context.Cache,
                    "IOptions`1" when paramType.IsGenericType => _context.JwtSettings,
                    nameof(IServiceProvider) or "IServiceProvider" => _context.ServiceProvider,
                    nameof(IUnitOfWork) or "IUnitOfWork" => _context.UnitOfWork,
                    _ => throw new InvalidOperationException(
                        $"Cannot resolve parameter '{parameters[i].Name}' of type '{paramType.Name}' " +
                        $"for repository '{implementationType.Name}'. " +
                        $"Supported types: TestContext, IDbConnection, IDbTransaction, IMemoryCache, " +
                        $"IOptions<JWTSetting>, IServiceProvider, IUnitOfWork")
                };
            }

            // Create instance
            return Activator.CreateInstance(implementationType, args)
                ?? throw new InvalidOperationException($"Failed to create instance of '{implementationType.Name}'");
        }

        /// <summary>
        /// Gets all discovered repository types (for debugging).
        /// </summary>
        public static IReadOnlyDictionary<Type, Type> GetDiscoveredRepositories()
        {
            return _repositoryRegistry.Value;
        }
    }
}
