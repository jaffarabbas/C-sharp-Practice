using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        // EF Core's default Guid generator on SQL Server is sequential (client-side), which keeps the
        // clustered primary key append-mostly. The value is assigned on Add(), before SaveChanges,
        // so an execution-strategy retry re-sends the same key instead of inserting a second row.
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Sku)
            .HasMaxLength(Product.SkuMaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(Product.NameMaxLength)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(Product.DescriptionMaxLength);

        builder.Property(p => p.Price)
            .HasPrecision(18, Product.PriceDecimalPlaces);

        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();

        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        // Business key: enforces SKU uniqueness under concurrency.
        builder.HasIndex(p => p.Sku)
            .IsUnique();

        // Supports the default list ordering (newest first, Id as tie-breaker).
        builder.HasIndex(p => new { p.CreatedAtUtc, p.Id })
            .IsDescending(true, true);
    }
}
