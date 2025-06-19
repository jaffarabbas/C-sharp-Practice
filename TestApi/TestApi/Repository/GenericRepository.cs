using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TestApi.Models;

namespace TestApi.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TestContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;

        public GenericRepository(TestContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        //dapper

        public async Task<IEnumerable<T>> GetAllAsync(string table)
        {
            var sql = $"SELECT * FROM {table}";
            return await _connection.QueryAsync<T>(sql, transaction: _transaction);
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
