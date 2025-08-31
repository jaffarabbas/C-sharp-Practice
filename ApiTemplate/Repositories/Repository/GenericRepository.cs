using ApiTemplate.Dtos;
using ApiTemplate.Helper.Enum;
using DBLayer.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace ApiTemplate.Repository
{
    public class GenericRepository<T> : IGenericRepositoryWrapper<T> where T : class
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

        #region Wrapper CRUD Methods

        public async Task<IEnumerable<T>> GetAllAsync(OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            return ormType switch
            {
                OrmType.EntityFramework => await GetAllAsync(),
                OrmType.Dapper => await GetAllAsync(options.TableName ?? typeof(T).Name),
                _ => throw new ArgumentException("Invalid ORM type")
            };
        }

        public async Task<T?> GetByIdAsync(object id, OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            return ormType switch
            {
                OrmType.EntityFramework => await GetByIdAsync(id),
                OrmType.Dapper => await GetByIdAsync(options.TableName ?? typeof(T).Name, options.KeyColumnName!, id),
                _ => throw new ArgumentException("Invalid ORM type")
            };
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            return ormType switch
            {
                OrmType.EntityFramework => await GetEnityPagedAsync(pageNumber, pageSize),
                OrmType.Dapper => await GetPagedDapperAsync(options.TableName ?? typeof(T).Name, pageNumber, pageSize, options.OrderBy!),
                _ => throw new ArgumentException("Invalid ORM type")
            };
        } 

        public async Task<object> AddAsync(T entity, OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            return ormType switch
            {
                OrmType.EntityFramework => await AddAsync(entity),
                OrmType.Dapper => await AddAsync(options.TableName ?? typeof(T).Name, entity),
                _ => throw new ArgumentException("Invalid ORM type")
            };
        }

        public async Task<bool> UpdateAsync(T entity, OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            try
            {
                switch (ormType)
                {
                    case OrmType.EntityFramework:
                        Update(entity);
                        return true;
                    case OrmType.Dapper:
                        var result = await UpdateAsync(options.TableName ?? typeof(T).Name, entity, options.KeyColumnName!);
                        return result > 0;
                    default:
                        throw new ArgumentException("Invalid ORM type");
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(object id, OrmType ormType, CrudOptions? options = null)
        {
            options ??= new CrudOptions();

            try
            {
                switch (ormType)
                {
                    case OrmType.EntityFramework:
                        var entity = await GetByIdAsync(id);
                        if (entity != null)
                        {
                            Delete(entity);
                            return true;
                        }
                        return false;
                    case OrmType.Dapper:
                        var result = await DeleteAsync(options.TableName ?? typeof(T).Name, options.KeyColumnName!, id);
                        return result > 0;
                    default:
                        throw new ArgumentException("Invalid ORM type");
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEntityAsync(T entity, OrmType ormType, CrudOptions? options = null)
        {
            try
            {
                switch (ormType)
                {
                    case OrmType.EntityFramework:
                        Delete(entity);
                        return true;
                    case OrmType.Dapper:
                        options ??= new CrudOptions();
                        // Get the ID value from the entity for Dapper deletion
                        var idProperty = typeof(T).GetProperty(options.KeyColumnName!);
                        if (idProperty != null)
                        {
                            var idValue = idProperty.GetValue(entity);
                            var result = await DeleteAsync(options.TableName ?? typeof(T).Name, options.KeyColumnName!, idValue!);
                            return result > 0;
                        }
                        return false;
                    default:
                        throw new ArgumentException("Invalid ORM type");
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Original Interface Implementation (EF Core + Cache)

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

        #endregion

        #region Original Interface Implementation (Dapper - No cache)

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
            var props = typeof(T).GetProperties()
                .Where(p =>
                    p.Name.ToLower() != "id" &&
                    p.CanRead &&
                    IsSimpleType(p.PropertyType)) // exclude navigation / complex / collections
                .ToList();

            var columns = string.Join(", ", props.Select(p => p.Name));
            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {table} ({columns}) VALUES ({parameters})";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> UpdateAsync(string table, T entity, string keyName)
        {
            var props = typeof(T).GetProperties()
                .Where(p =>
                    p.Name != keyName &&
                    p.CanRead &&
                    IsSimpleType(p.PropertyType))
                .ToList();

            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
            var sql = $"UPDATE {table} SET {setClause} WHERE {keyName} = @{keyName}";
            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> DeleteAsync(string table, string keyName, object id)
        {
            var sql = $"DELETE FROM {table} WHERE {keyName} = @id";
            return await _connection.ExecuteAsync(sql, new { id }, _transaction);
        }

        public async Task<long> GetMaxID(string tableName, string columnName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name must be provided", nameof(tableName));

            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("Key column name must be provided", nameof(columnName));

            var sql = $"SELECT ISNULL(MAX([{columnName}]), 0) FROM [{tableName}]";

            var maxId = await _connection.ExecuteScalarAsync<long>(sql, transaction: _transaction);
            return maxId + 1;
        }

        #endregion

        #region Helper Methods for Wrapper

        private async Task<PagedResult<T>> GetPagedDapperAsync(string table, int pageNumber, int pageSize, string orderBy)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 100;

            if (string.IsNullOrWhiteSpace(orderBy))
                throw new ArgumentException("You must provide a valid ORDER BY column for SQL Server OFFSET/FETCH to work.");

            int offset = (pageNumber - 1) * pageSize;

            var countSql = $"SELECT COUNT(*) FROM {table}";
            var totalCount = await _connection.QuerySingleAsync<int>(countSql, transaction: _transaction);

            var sql = $@"SELECT * FROM {table}
                        ORDER BY {orderBy}
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY";

            var items = await _connection.QueryAsync<T>(sql, new { Offset = offset, PageSize = pageSize }, _transaction);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type)!;

            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(Guid)
                   || type == typeof(byte[])
                   || type == typeof(bool)
                   || type == typeof(double)
                   || type == typeof(float);
        }

        #endregion

    }
}
