using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Application.Products;

// Price is nullable so a missing value is a validation error instead of silently becoming 0.
public sealed record CreateProductRequest(string? Sku, string? Name, string? Description, decimal? Price);

/// <param name="Version">The <see cref="ProductResponse.Version"/> the client last read (optimistic concurrency).</param>
public sealed record UpdateProductRequest(string? Name, string? Description, decimal? Price, string? Version);

public sealed record ProductResponse(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    string Version)
{
    public static ProductResponse From(Product product) => new(
        product.Id,
        product.Sku,
        product.Name,
        product.Description,
        product.Price,
        product.CreatedAtUtc,
        product.UpdatedAtUtc,
        Convert.ToBase64String(product.RowVersion));
}
