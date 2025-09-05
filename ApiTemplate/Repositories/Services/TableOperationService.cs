using ApiTemplate.Helper.Enum;
using ApiTemplate.Repository;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.Dtos;
using Shared.Helper.Enum;
using System.Reflection;
using System.Text.Json;
    
namespace Repositories.Services
{
    public class TableOperationService : ITableOperationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TestContext _context;
        private static readonly Dictionary<string, Type> TableTypeCache = new(StringComparer.OrdinalIgnoreCase);

        public TableOperationService(IUnitOfWork unitOfWork, TestContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            CacheEntities();
        }

        #region Public Convenience API (Used by Controller)
        public async Task<object?> GetAllAsync(string tableName, OrmType orm, int? pageNumber, int? pageSize, string? orderBy)
        {
            var isPaged = pageNumber.HasValue || pageSize.HasValue;
            if (!isPaged)
            {
                var req = new TableOperationRequest
                {
                    TableName = tableName,
                    OrmType = orm,
                    Operation = TableOperationType.GetAll
                };
                return await ExecuteAsync(req);
            }

            var pagedReq = new TableOperationRequest
            {
                TableName = tableName,
                OrmType = orm,
                Operation = TableOperationType.Paged,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 50,
                OrderBy = orderBy
            };
            return await ExecuteAsync(pagedReq);
        }

        public async Task<object?> GetByIdAsync(string tableName, object id, OrmType orm)
        {
            var req = new TableOperationRequest
            {
                TableName = tableName,
                OrmType = orm,
                Operation = TableOperationType.GetById,
                Id = id
            };
            return await ExecuteAsync(req);
        }

        public async Task<object?> AddAsync(string tableName, JsonElement jsonBody, OrmType orm)
        {
            var data = ToDictionary(jsonBody);
            var req = new TableOperationRequest
            {
                TableName = tableName,
                OrmType = orm,
                Operation = TableOperationType.Add,
                Data = data
            };
            var result = await ExecuteAsync(req);
            await CommitIfNeededAsync(req);
            return result;
        }

        public async Task<object?> UpdateAsync(string tableName, object id, JsonElement jsonBody, OrmType orm)
        {
            var data = ToDictionary(jsonBody);
            var req = new TableOperationRequest
            {
                TableName = tableName,
                OrmType = orm,
                Operation = TableOperationType.Update,
                Id = id,
                Data = data
            };
            var result = await ExecuteAsync(req);
            if (result != null)
                await CommitIfNeededAsync(req);
            return result;
        }

        public async Task<bool> DeleteAsync(string tableName, object id, OrmType orm)
        {
            var req = new TableOperationRequest
            {
                TableName = tableName,
                OrmType = orm,
                Operation = TableOperationType.Delete,
                Id = id
            };
            var result = await ExecuteAsync(req);
            if (result is bool success && success)
            {
                await CommitIfNeededAsync(req);
                return true;
            }
            return false;
        }
        #endregion

        #region Core Generic Execution (Existing Logic)
        public async Task<object?> ExecuteAsync(TableOperationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
                throw new ArgumentException("TableName is required.");

            if (!TableTypeCache.TryGetValue(request.TableName, out var entityType))
                throw new InvalidOperationException($"Table '{request.TableName}' not mapped to an entity.");

            var repo = GetRepository(entityType);
            var keyColumn = request.KeyColumnName ?? GetPrimaryKeyColumn(entityType, request.TableName);

            return request.Operation switch
            {
                TableOperationType.GetAll => await GetAllInternalAsync(repo, request),
                TableOperationType.GetById => await GetByIdInternalAsync(repo, request, keyColumn),
                TableOperationType.Add => await AddInternalAsync(repo, entityType, request),
                TableOperationType.Update => await UpdateInternalAsync(repo, entityType, request, keyColumn),
                TableOperationType.Delete => await DeleteInternalAsync(repo, request, keyColumn),
                TableOperationType.Paged => await PagedInternalAsync(repo, request, keyColumn),
                _ => throw new NotSupportedException("Unsupported operation.")
            };
        }
        #endregion

        #region Internal Helpers
        private void CacheEntities()
        {
            if (TableTypeCache.Count > 0) return;
            foreach (var et in _context.Model.GetEntityTypes())
            {
                var tableName = et.GetTableName();
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    TableTypeCache[tableName] = et.ClrType;
                }
            }
        }

        private object GetRepository(Type entityType)
        {
            var method = typeof(IUnitOfWork).GetMethod(nameof(IUnitOfWork.Repository))!;
            return method.MakeGenericMethod(entityType).Invoke(_unitOfWork, null)!;
        }

        private string GetPrimaryKeyColumn(Type entityType, string tableName)
        {
            var et = _context.Model.GetEntityTypes().First(e => e.ClrType == entityType);
            var pk = et.FindPrimaryKey() ?? throw new InvalidOperationException($"No PK for {entityType.Name}");
            var storeId = StoreObjectIdentifier.Table(tableName, null);
            var prop = pk.Properties.First();
            return prop.GetColumnName(storeId) ?? prop.Name;
        }

        private async Task<object?> GetAllInternalAsync(object repo, TableOperationRequest req)
        {
            if (req.OrmType == OrmType.Dapper)
            {
                var m = repo.GetType().GetMethod("GetAllAsync", new[] { typeof(string) })!;
                return await (dynamic)m.Invoke(repo, new object[] { req.TableName })!;
            }
            var mEf = repo.GetType().GetMethod("GetAllAsync", Type.EmptyTypes)!;
            return await (dynamic)mEf.Invoke(repo, Array.Empty<object>());
        }

        private async Task<object?> GetByIdInternalAsync(object repo, TableOperationRequest req, string keyColumn)
        {
            if (req.Id == null) throw new ArgumentException("Id required.");
            if (req.OrmType == OrmType.Dapper)
            {
                var m = repo.GetType().GetMethod("GetByIdAsync", new[] { typeof(string), typeof(string), typeof(object) })!;
                return await (dynamic)m.Invoke(repo, new object[] { req.TableName, keyColumn, req.Id })!;
            }
            var mEf = repo.GetType().GetMethod("GetByIdAsync", new[] { typeof(object) })!;
            return await (dynamic)mEf.Invoke(repo, new object[] { req.Id })!;
        }

        private async Task<object?> AddInternalAsync(object repo, Type entityType, TableOperationRequest req)
        {
            if (req.Data == null) throw new ArgumentException("Data required for Add.");
            var entity = BuildEntity(entityType, req.Data);
            if (req.OrmType == OrmType.Dapper)
            {
                var m = repo.GetType().GetMethod("AddAsync", new[] { typeof(string), entityType })!;
                await (dynamic)m.Invoke(repo, new object[] { req.TableName, entity });
                return entity;
            }
            var mEf = repo.GetType().GetMethod("AddAsync", new[] { entityType })!;
            return await (dynamic)mEf.Invoke(repo, new object[] { entity })!;
        }

        private async Task<object?> UpdateInternalAsync(object repo, Type entityType, TableOperationRequest req, string keyColumn)
        {
            if (req.Data == null) throw new ArgumentException("Data required for Update.");

            object? id = req.Id;
            if (id == null)
            {
                var found = req.Data.FirstOrDefault(kv => string.Equals(kv.Key, keyColumn, StringComparison.OrdinalIgnoreCase));
                id = found.Value;
            }
            if (id == null) throw new ArgumentException("Id not provided.");

            object? existing;
            if (req.OrmType == OrmType.Dapper)
            {
                var gm = repo.GetType().GetMethod("GetByIdAsync", new[] { typeof(string), typeof(string), typeof(object) })!;
                existing = await (dynamic)gm.Invoke(repo, new object[] { req.TableName, keyColumn, id })!;
            }
            else
            {
                var gm = repo.GetType().GetMethod("GetByIdAsync", new[] { typeof(object) })!;
                existing = await (dynamic)gm.Invoke(repo, new object[] { id })!;
            }

            if (existing == null) return null;

            ApplyData(existing, req.Data);

            if (req.OrmType == OrmType.Dapper)
            {
                var um = repo.GetType().GetMethod("UpdateAsync", new[] { typeof(string), entityType, typeof(string) })!;
                await (dynamic)um.Invoke(repo, new object[] { req.TableName, existing, keyColumn });
            }
            else
            {
                var um = repo.GetType().GetMethod("Update", new[] { entityType })!;
                um.Invoke(repo, new object[] { existing });
            }

            return existing;
        }

        private async Task<object?> DeleteInternalAsync(object repo, TableOperationRequest req, string keyColumn)
        {
            if (req.Id == null) throw new ArgumentException("Id required for Delete.");
            if (req.OrmType == OrmType.Dapper)
            {
                var m = repo.GetType().GetMethod("DeleteAsync", new[] { typeof(string), typeof(string), typeof(object) })!;
                await (dynamic)m.Invoke(repo, new object[] { req.TableName, keyColumn, req.Id });
                return true;
            }
            var get = repo.GetType().GetMethod("GetByIdAsync", new[] { typeof(object) })!;
            var entity = await (dynamic)get.Invoke(repo, new object[] { req.Id })!;
            if (entity == null) return false;
            var del = repo.GetType().GetMethod("Delete", (Type[])(new[] { entity.GetType() }))!;
            del.Invoke(repo, new object[] { entity });
            return true;
        }

        private async Task<object?> PagedInternalAsync(object repo, TableOperationRequest req, string keyColumn)
        {
            if (req.OrmType == OrmType.Dapper)
            {
                var m = repo.GetType().GetMethod("GetPagedAsync", new[] { typeof(string), typeof(int), typeof(int), typeof(string) })!;
                var orderBy = string.IsNullOrWhiteSpace(req.OrderBy) ? keyColumn : req.OrderBy!;
                return await (dynamic)m.Invoke(repo, new object[] { req.TableName, req.PageNumber, req.PageSize, orderBy });
            }
            var mEf = repo.GetType().GetMethod("GetEnityPagedAsync", new[] { typeof(int), typeof(int) })!;
            return await (dynamic)mEf.Invoke(repo, new object[] { req.PageNumber, req.PageSize });
        }

        private object BuildEntity(Type entityType, Dictionary<string, object?> data)
        {
            var entity = Activator.CreateInstance(entityType)!;
            ApplyData(entity, data);
            return entity;
        }

        private void ApplyData(object target, Dictionary<string, object?> data)
        {
            var props = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var p in props)
            {
                var kv = data.FirstOrDefault(d => string.Equals(d.Key, p.Name, StringComparison.OrdinalIgnoreCase));
                if (kv.Key == null) continue;

                try
                {
                    if (kv.Value == null)
                    {
                        p.SetValue(target, null);
                        continue;
                    }
                    var destType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var converted = Convert.ChangeType(kv.Value, destType);
                    p.SetValue(target, converted);
                }
                catch
                {
                    // swallow conversion problems
                }
            }
        }

        private static Dictionary<string, object?> ToDictionary(JsonElement element)
        {
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (element.ValueKind != JsonValueKind.Object) return dict;

            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Number => prop.Value.TryGetInt64(out var l) ? l :
                                             prop.Value.TryGetDouble(out var d) ? d :
                                             prop.Value.GetRawText(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.String => prop.Value.GetString(),
                    _ => prop.Value.GetRawText()
                };
            }
            return dict;
        }

        private async Task CommitIfNeededAsync(TableOperationRequest req)
        {
            if (req.Operation is TableOperationType.Add or TableOperationType.Update or TableOperationType.Delete)
            {
                if (req.OrmType == OrmType.EntityFramework)
                    await _unitOfWork.SaveAsync();
                else
                    _unitOfWork.Commit();
            }
        }
        #endregion
    }
}