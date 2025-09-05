using System.Text.Json.Serialization;
using Shared.Helper.Enum;
using ApiTemplate.Helper.Enum;

namespace Shared.Dtos
{
    public class TableOperationRequest
    {
        public string TableName { get; set; } = null!;
        public TableOperationType Operation { get; set; } = TableOperationType.GetAll;
        public object? Id { get; set; }
        public string? KeyColumnName { get; set; }
        public Dictionary<string, object?>? Data { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize  { get; set; } = 50;
        public string? OrderBy { get; set; }
        public OrmType OrmType { get; set; } = OrmType.Dapper;
    }
}