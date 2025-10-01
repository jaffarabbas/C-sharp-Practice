using ApiTemplate.Dtos;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repositories.Factory;
using Repositories.Infrastructure;
using Repositories.Repository;
using Repositories.Services;
using System.Collections;
using System.Data;

namespace ApiTemplate.Repository
{
    /// <summary>
    /// ULTIMATE CLEAN UnitOfWork - SINGLE dependency, maximum maintainability.
    /// Uses RepositoryFactory with auto-discovery - NO manual repository registration needed!
    /// Uses RepositoryContext to consolidate ALL infrastructure dependencies.
    /// Both EF Core (TestContext) and Dapper (IDbConnection) treated equally - resolved internally!
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TestContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IServiceProvider _serviceProvider;

        private Hashtable _repositories;
        private IDbContextTransaction? _efTransaction;
        private IDbConnection _connection;
        private IDbTransaction _dapperTransaction;

        private ITableOperationService? _tableOps;

        /// <summary>
        /// ULTIMATE CLEAN constructor - ONLY ONE PARAMETER!
        /// Everything resolved internally from IServiceProvider.
        /// Both EF (TestContext) and Dapper (IDbConnection) treated equally.
        /// </summary>
        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Resolve ALL infrastructure dependencies internally
            _context = _serviceProvider.GetRequiredService<TestContext>();
            var cache = _serviceProvider.GetRequiredService<IMemoryCache>();
            var jwtSettings = _serviceProvider.GetRequiredService<IOptions<JWTSetting>>();
            _connection = _serviceProvider.GetRequiredService<IDbConnection>();

            // Open connection and start transaction
            _connection.Open();
            _dapperTransaction = _connection.BeginTransaction();

            // Create complete repository context (consolidates everything)
            var repositoryContext = new RepositoryContext(
                _context,
                _connection,
                _dapperTransaction,
                cache,
                jwtSettings,
                _serviceProvider,
                this);

            // Create repository factory with auto-discovery
            _repositoryFactory = new RepositoryFactory(repositoryContext);
        }

        #region Modern Repository Access (Auto-Discovery)

        /// <summary>
        /// Gets a repository instance using auto-discovery.
        /// Repositories are automatically discovered via [AutoRegisterRepository] attribute.
        /// NO MANUAL REGISTRATION NEEDED!
        /// Example: var authRepo = _unitOfWork.GetRepository&lt;IAuthRepository&gt;();
        /// </summary>
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            return _repositoryFactory.GetRepository<TRepository>();
        }

        #endregion

        #region Legacy Repository Access (Deprecated - for backward compatibility)

        [Obsolete("Use GetRepository<IAuthRepository>() instead")]
        public IAuthRepository IAuthRepository => GetRepository<IAuthRepository>();

        [Obsolete("Use GetRepository<IITemRepository>() instead")]
        public IITemRepository iTemRepository => GetRepository<IITemRepository>();

        #endregion

        #region Generic Repository Access (EF/Dapper direct access)

        public IGenericRepository<T> Repository<T>() where T : class
        {
            _repositories ??= new Hashtable();
            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                // Resolve infrastructure from service provider
                var cache = _serviceProvider.GetRequiredService<IMemoryCache>();

                var repoInstance = new GenericRepository<T>(
                    _context,
                    _connection,
                    cache,
                    _dapperTransaction);
                _repositories.Add(type, repoInstance);
            }

            return (IGenericRepository<T>)_repositories[type]!;
        }

        public IGenericRepositoryWrapper<T> RepositoryWrapper<T>() where T : class
        {
            _repositories ??= new Hashtable();
            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                // Resolve infrastructure from service provider
                var cache = _serviceProvider.GetRequiredService<IMemoryCache>();

                var repoInstance = new GenericRepository<T>(
                    _context,
                    _connection,
                    cache,
                    _dapperTransaction);
                _repositories.Add(type, repoInstance);
            }

            return (IGenericRepositoryWrapper<T>)_repositories[type]!;
        }

        #endregion

        #region Dynamic Table Operations

        public ITableOperationService TableOperations =>
            _tableOps ??= _serviceProvider.GetRequiredService<ITableOperationService>();

        #endregion

        #region Transaction Management (EF Core)

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            if (_efTransaction == null)
                _efTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.CommitAsync();
                await _efTransaction.DisposeAsync();
                _efTransaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                await _efTransaction.DisposeAsync();
                _efTransaction = null;
            }
        }

        #endregion

        #region Transaction Management (Dapper)

        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _dapperTransaction;

        public void Commit()
        {
            _dapperTransaction.Commit();
            _dapperTransaction.Dispose();
            _dapperTransaction = _connection.BeginTransaction();
        }

        public void Rollback()
        {
            _dapperTransaction.Rollback();
            _dapperTransaction.Dispose();
            _dapperTransaction = _connection.BeginTransaction();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _efTransaction?.Dispose();
            _dapperTransaction?.Dispose();
            _connection?.Dispose();
            _context.Dispose();
        }

        #endregion
    }
}
