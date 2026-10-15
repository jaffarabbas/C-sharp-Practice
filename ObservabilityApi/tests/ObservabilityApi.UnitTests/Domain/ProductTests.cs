using ObservabilityApi.Domain;
using ObservabilityApi.Domain.Products;

namespace ObservabilityApi.UnitTests.Domain;

public sealed class ProductTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_normalizes_sku_and_trims_text()
    {
        var product = Product.Create("  ab-123 ", "  Widget  ", "   ", 9.99m, Now);

        Assert.Equal("AB-123", product.Sku);
        Assert.Equal("Widget", product.Name);
        Assert.Null(product.Description);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(Now, product.CreatedAtUtc);
        Assert.Equal(Now, product.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("-ABC")]
    [InlineData("AB_12")]
    [InlineData("AB 12")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456")] // 33 characters
    public void Create_rejects_invalid_sku(string sku)
    {
        Assert.Throws<DomainException>(() => Product.Create(sku, "Widget", null, 1m, Now));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1_000_000.01)]
    [InlineData(1.001)]
    public void Create_rejects_invalid_price(double price)
    {
        Assert.Throws<DomainException>(() => Product.Create("SKU-1", "Widget", null, (decimal)price, Now));
    }

    [Fact]
    public void Create_rejects_name_longer_than_limit()
    {
        var name = new string('x', Product.NameMaxLength + 1);
        Assert.Throws<DomainException>(() => Product.Create("SKU-1", name, null, 1m, Now));
    }

    [Fact]
    public void Update_changes_details_and_timestamp_but_not_sku_or_creation_time()
    {
        var product = Product.Create("SKU-1", "Widget", null, 1m, Now);
        var later = Now.AddMinutes(5);

        product.Update("Widget Pro", "Better", 2.50m, later);

        Assert.Equal("SKU-1", product.Sku);
        Assert.Equal("Widget Pro", product.Name);
        Assert.Equal("Better", product.Description);
        Assert.Equal(2.50m, product.Price);
        Assert.Equal(Now, product.CreatedAtUtc);
        Assert.Equal(later, product.UpdatedAtUtc);
    }

    [Fact]
    public void Failed_update_leaves_product_unchanged()
    {
        var product = Product.Create("SKU-1", "Widget", null, 1m, Now);

        Assert.Throws<DomainException>(() => product.Update("Widget", null, -5m, Now.AddMinutes(1)));

        Assert.Equal(1m, product.Price);
        Assert.Equal(Now, product.UpdatedAtUtc);
    }
}
