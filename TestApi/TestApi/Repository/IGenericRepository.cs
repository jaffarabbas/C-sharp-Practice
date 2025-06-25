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
        Task<IEnumerable<T>> GetAllAsync(string table);
        Task<PagedResult<T>> GetEnityPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetPagedAsync(string table, int pageNumber, int pageSize, string orderBy = "1");
        Task<T?> GetByIdAsync(string table, string keyName, object id);
        Task<int> AddAsync(string table, T entity);
        Task<int> UpdateAsync(string table, T entity, string keyName);
        Task<int> DeleteAsync(string table, string keyName, object id);
    }



}
