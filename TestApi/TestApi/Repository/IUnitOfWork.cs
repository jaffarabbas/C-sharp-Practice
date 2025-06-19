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
    }

}
