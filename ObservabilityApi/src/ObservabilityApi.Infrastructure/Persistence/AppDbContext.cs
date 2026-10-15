using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ObservabilityApi.Application.Abstractions;
using ObservabilityApi.Application.Common;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    /// <summary>
    /// All objects owned by this service live in their own schema, so it can share a database
    /// (e.g. the local <c>test2</c> database) without colliding with other applications' tables,
    /// including their <c>dbo.__EFMigrationsHistory</c>.
    /// </summary>
    public const string Schema = "catalog";

    private const int SqlUniqueIndexViolation = 2601;
    private const int SqlUniqueConstraintViolation = 2627;

    public DbSet<Product> Products => Set<Product>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConflictException("The product was modified or deleted by another request. Re-read it and retry.", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: SqlUniqueIndexViolation or SqlUniqueConstraintViolation })
        {
            throw new ConflictException("The request conflicts with an existing product (unique key violation).", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
