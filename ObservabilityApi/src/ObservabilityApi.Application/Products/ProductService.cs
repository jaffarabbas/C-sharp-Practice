using System.Diagnostics;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ObservabilityApi.Application.Abstractions;
using ObservabilityApi.Application.Common;
using ObservabilityApi.Application.Telemetry;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Application.Products;

/// <summary>
/// Product use cases. Returns <c>null</c>/<c>false</c> for "not found" (an expected outcome) and throws
/// <see cref="ValidationException"/> or <see cref="ConflictException"/> for rejected requests, which the
/// API maps to ProblemDetails in one place.
/// </summary>
public sealed class ProductService(
    IAppDbContext db,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateProductRequest> updateValidator,
    IValidator<PageRequest> pageValidator,
    ProductMetrics metrics,
    TimeProvider clock,
    ILogger<ProductService> logger)
{
    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("CreateProduct");
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var sku = Product.NormalizeSku(request.Sku);
        activity?.SetTag("product.sku", sku);

        // Fast path for the common duplicate case. The unique index is the real guarantee under
        // concurrency; Infrastructure translates its violation into the same ConflictException.
        if (await db.Products.AnyAsync(p => p.Sku == sku, cancellationToken))
        {
            ProductLog.DuplicateSkuRejected(logger, sku);
            throw new ConflictException($"A product with SKU '{sku}' already exists.");
        }

        var product = Product.Create(sku, request.Name!, request.Description, request.Price!.Value, clock.GetUtcNow());
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        activity?.SetTag("product.id", product.Id);
        metrics.ProductCreated();
        ProductLog.ProductCreated(logger, product.Id, product.Sku);
        return ProductResponse.From(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("GetProduct");
        activity?.SetTag("product.id", id);

        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        activity?.SetTag("product.found", product is not null);
        return product is null ? null : ProductResponse.From(product);
    }

    public async Task<PagedResult<ProductResponse>> ListAsync(PageRequest page, CancellationToken cancellationToken)
    {
        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("ListProducts");
        await pageValidator.ValidateAndThrowAsync(page, cancellationToken);
        activity?.SetTag("page.number", page.Page);
        activity?.SetTag("page.size", page.PageSize);

        var query = db.Products.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        // Newest first; Id breaks ties so paging is deterministic. Backed by IX_Products_CreatedAtUtc_Id.
        var products = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .ThenByDescending(p => p.Id)
            .Skip(page.Skip)
            .Take(page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductResponse>(
            products.ConvertAll(ProductResponse.From), page.Page, page.PageSize, totalCount);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("UpdateProduct");
        activity?.SetTag("product.id", id);
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        // Reject updates based on a stale read. A concurrent write between this check and SaveChanges
        // is caught by the rowversion concurrency token and also surfaces as a ConflictException.
        RowVersion.TryDecode(request.Version, out var clientVersion);
        if (!product.RowVersion.AsSpan().SequenceEqual(clientVersion))
        {
            ProductLog.StaleVersionRejected(logger, id);
            activity?.SetTag("product.conflict", "stale_version");
            throw new ConflictException("The product was modified by another request. Re-read it and retry with the new version.");
        }

        product.Update(request.Name!, request.Description, request.Price!.Value, clock.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);

        metrics.ProductUpdated();
        ProductLog.ProductUpdated(logger, id);
        return ProductResponse.From(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using var activity = ApplicationTelemetry.ActivitySource.StartActivity("DeleteProduct");
        activity?.SetTag("product.id", id);

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);

        metrics.ProductDeleted();
        ProductLog.ProductDeleted(logger, id);
        return true;
    }
}
