using System.Data;

namespace TestApi.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IITemRepository iTemRepository { get; }
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> SaveAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        //dapper
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();
    }

}
