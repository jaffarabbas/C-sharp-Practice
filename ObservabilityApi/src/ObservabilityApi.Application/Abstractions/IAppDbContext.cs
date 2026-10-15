using Microsoft.EntityFrameworkCore;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Application.Abstractions;

/// <summary>
/// The unit of work the application layer needs. Deliberately exposes EF Core's DbSet instead of a
/// generic repository: DbSet already is a repository, and LINQ keeps queries (paging, projections) simple.
/// Implementations must translate provider-specific constraint and concurrency failures into
/// <see cref="Common.ConflictException"/>.
/// </summary>
public interface IAppDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
