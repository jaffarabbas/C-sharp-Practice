namespace ApiTemplate.Dtos
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class CrudOptions
    {
        public string? TableName { get; set; }
        public string? KeyColumnName { get; set; } = "";
        public string? OrderBy { get; set; } = "";
        public bool UseCache { get; set; } = true;
    }
}
