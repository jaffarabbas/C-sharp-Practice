using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using TestApi.Models;

namespace TestApi.Repository
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TestContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;
        private readonly IMemoryCache _cache;

        public GenericRepository(TestContext context, IDbConnection connection, IMemoryCache cache, IDbTransaction? transaction = null)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _connection = connection;
            _transaction = transaction;
            _cache = cache;
        }

        // EF Core + Cache

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var cacheKey = $"All_{typeof(T).Name}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<T> cachedList))
            {
                return cachedList;
            }

            var data = await _dbSet.ToListAsync();
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
            return data;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            var cacheKey = $"{typeof(T).Name}_Id_{id}";
            if (_cache.TryGetValue(cacheKey, out T cachedEntity))
            {
                return cachedEntity;
            }

            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(10));
            }
            return entity;
        }

        public async Task<PagedResult<T>> GetEnityPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var cacheKey = $"{typeof(T).Name}_Page_{pageNumber}_Size_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out PagedResult<T> cachedPage))
            {
                return cachedPage;
            }

            var totalCount = await _dbSet.CountAsync();
            var items = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            // Invalidate list cache
            _cache.Remove($"All_{typeof(T).Name}");
            return result.Entity;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _cache.Remove($"All_{typeof(T).Name}");
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _cache.Remove($"All_{typeof(T).Name}");
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Dapper (No cache applied here to keep it raw/flexible)

        public async Task<IEnumerable<T>> GetAllAsync(string table)
        {
            var sql = $"SELECT * FROM {table}";
            return await _connection.QueryAsync<T>(sql, transaction: _transaction);
        }

        public async Task<IEnumerable<T>> GetPagedAsync(string table, int pageNumber, int pageSize, string orderBy = "1")
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 100;

            int offset = (pageNumber - 1) * pageSize;

            var sql = $"SELECT * FROM {table} ORDER BY {orderBy} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            return await _connection.QueryAsync<T>(sql, new { Offset = offset, PageSize = pageSize }, _transaction);
        }

        public async Task<T?> GetByIdAsync(string table, string keyName, object id)
        {
            var sql = $"SELECT * FROM {table} WHERE {keyName} = @id";
            return await _connection.QueryFirstOrDefaultAsync<T>(sql, new { id }, _transaction);
        }

        public async Task<int> AddAsync(string table, T entity)
        {
            var props = typeof(T).GetProperties().Where(p => p.Name.ToLower() != "id").ToList();
            var columns = string.Join(", ", props.Select(p => p.Name));
            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {table} ({columns}) VALUES ({parameters})";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> UpdateAsync(string table, T entity, string keyName)
        {
            var props = typeof(T).GetProperties().Where(p => p.Name != keyName).ToList();
            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

            var sql = $"UPDATE {table} SET {setClause} WHERE {keyName} = @{keyName}";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> DeleteAsync(string table, string keyName, object id)
        {
            var sql = $"DELETE FROM {table} WHERE {keyName} = @id";
            return await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }
    }
}
