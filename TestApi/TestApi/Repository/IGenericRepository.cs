namespace TestApi.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<T> AddAsync(T entity); // return added entity
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }



}
