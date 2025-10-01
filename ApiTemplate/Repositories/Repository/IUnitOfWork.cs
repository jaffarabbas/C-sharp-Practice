using Repositories.Repository;
using Repositories.Services;              // + added
using System.Data;

namespace ApiTemplate.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        // Modern approach - resolve repositories dynamically via factory
        TRepository GetRepository<TRepository>() where TRepository : class;

        // Legacy support - kept for backward compatibility (will be deprecated)
        [Obsolete("Use GetRepository<IAuthRepository>() instead")]
        IAuthRepository IAuthRepository { get; }

        [Obsolete("Use GetRepository<IITemRepository>() instead")]
        IITemRepository iTemRepository { get; }

        // Generic repository access
        IGenericRepository<T> Repository<T>() where T : class;
        IGenericRepositoryWrapper<T> RepositoryWrapper<T>() where T : class;

        // Dynamic table operations
        ITableOperationService TableOperations { get; }

        // Transaction management
        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        // Dapper transaction management
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();
    }
}
