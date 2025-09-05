using Shared.Dtos;
using ApiTemplate.Helper.Enum;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories.Services
{
    // Extended interface so controller only forwards parameters.
    public interface ITableOperationService
    {
        // Original generic executor (kept for advanced scenarios)
        Task<object?> ExecuteAsync(TableOperationRequest request);

        // Convenience methods (new)
        Task<object?> GetAllAsync(string tableName, OrmType orm, int? pageNumber, int? pageSize, string? orderBy);
        Task<object?> GetByIdAsync(string tableName, object id, OrmType orm);
        Task<object?> AddAsync(string tableName, JsonElement jsonBody, OrmType orm);
        Task<object?> UpdateAsync(string tableName, object id, JsonElement jsonBody, OrmType orm);
        Task<bool> DeleteAsync(string tableName, object id, OrmType orm);
    }
}