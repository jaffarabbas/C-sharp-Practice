using Microsoft.Extensions.Logging;

namespace ObservabilityApi.Application.Products;

/// <summary>
/// Source-generated, structured log events. Stable EventIds/names make events queryable in Seq
/// (for example <c>EventId.Name = 'ProductCreated'</c>) independent of the message wording.
/// </summary>
internal static partial class ProductLog
{
    [LoggerMessage(EventId = 1001, EventName = "ProductCreated", Level = LogLevel.Information,
        Message = "Product {ProductId} created with SKU {Sku}")]
    public static partial void ProductCreated(ILogger logger, Guid productId, string sku);

    [LoggerMessage(EventId = 1002, EventName = "ProductUpdated", Level = LogLevel.Information,
        Message = "Product {ProductId} updated")]
    public static partial void ProductUpdated(ILogger logger, Guid productId);

    [LoggerMessage(EventId = 1003, EventName = "ProductDeleted", Level = LogLevel.Information,
        Message = "Product {ProductId} deleted")]
    public static partial void ProductDeleted(ILogger logger, Guid productId);

    [LoggerMessage(EventId = 1101, EventName = "DuplicateSkuRejected", Level = LogLevel.Information,
        Message = "Product creation rejected: SKU {Sku} already exists")]
    public static partial void DuplicateSkuRejected(ILogger logger, string sku);

    [LoggerMessage(EventId = 1102, EventName = "StaleVersionRejected", Level = LogLevel.Information,
        Message = "Update of product {ProductId} rejected: client version is stale")]
    public static partial void StaleVersionRejected(ILogger logger, Guid productId);
}
