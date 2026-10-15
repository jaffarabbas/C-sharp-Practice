using ObservabilityApi.Application.Common;
using ObservabilityApi.Application.Products;

namespace ObservabilityApi.UnitTests.Application;

public sealed class ValidatorTests
{
    private static readonly string ValidVersion = Convert.ToBase64String(new byte[8]);

    private readonly CreateProductRequestValidator create = new();
    private readonly UpdateProductRequestValidator update = new();
    private readonly PageRequestValidator page = new();

    [Fact]
    public void Valid_create_request_passes()
    {
        var result = create.Validate(new CreateProductRequest("sku-1", "Widget", null, 10.50m));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Create_request_reports_every_invalid_field()
    {
        var result = create.Validate(new CreateProductRequest("!", " ", new string('d', 2001), null));

        var fields = result.Errors.Select(e => e.PropertyName).Distinct().Order().ToArray();
        Assert.Equal(["Description", "Name", "Price", "Sku"], fields);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1_000_001)]
    [InlineData(1.234)]
    public void Create_request_rejects_invalid_price(double price)
    {
        var result = create.Validate(new CreateProductRequest("SKU-1", "Widget", null, (decimal)price));
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void Error_messages_do_not_echo_submitted_values()
    {
        const string secretLikeInput = "super-secret-value-123";
        var result = create.Validate(new CreateProductRequest(secretLikeInput + "!", secretLikeInput + new string('x', 200), null, -1m));

        Assert.All(result.Errors, e => Assert.DoesNotContain(secretLikeInput, e.ErrorMessage, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-base64!")]
    [InlineData("AQID")] // valid base64, but 3 bytes instead of 8
    public void Update_request_requires_a_well_formed_version(string? version)
    {
        var result = update.Validate(new UpdateProductRequest("Widget", null, 1m, version));
        Assert.Contains(result.Errors, e => e.PropertyName == "Version");
    }

    [Fact]
    public void Valid_update_request_passes()
    {
        Assert.True(update.Validate(new UpdateProductRequest("Widget", "Desc", 1m, ValidVersion)).IsValid);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, PageRequest.MaxPageSize + 1)]
    [InlineData(PageRequest.MaxPage + 1, 20)]
    public void Page_request_is_bounded(int pageNumber, int pageSize)
    {
        Assert.False(page.Validate(new PageRequest(pageNumber, pageSize)).IsValid);
    }

    [Fact]
    public void Page_request_skip_is_computed_from_page_and_size()
    {
        Assert.Equal(40, new PageRequest(3, 20).Skip);
    }
}
