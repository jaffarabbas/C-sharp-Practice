using Microsoft.Extensions.Primitives;
using ObservabilityApi.Api.Correlation;

namespace ObservabilityApi.UnitTests.Api;

public sealed class CorrelationIdTests
{
    [Theory]
    [InlineData("abc-123")]
    [InlineData("order:42.retry_1")]
    [InlineData("0123456789012345678901234567890123456789012345678901234567890123")] // 64 characters
    public void Accepts_well_formed_ids(string value)
    {
        var resolved = CorrelationIdMiddleware.ResolveCorrelationId(new StringValues(value), out var rejected);

        Assert.Equal(value, resolved);
        Assert.False(rejected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("has space")]
    [InlineData("line\nbreak")]
    [InlineData("<script>")]
    [InlineData("01234567890123456789012345678901234567890123456789012345678901234")] // 65 characters
    public void Replaces_malformed_ids_with_a_generated_one(string value)
    {
        var resolved = CorrelationIdMiddleware.ResolveCorrelationId(new StringValues(value), out var rejected);

        Assert.NotEqual(value, resolved);
        Assert.True(rejected);
        Assert.True(Guid.TryParseExact(resolved, "D", out _));
    }

    [Fact]
    public void Generates_an_id_when_header_is_missing_without_flagging_rejection()
    {
        var resolved = CorrelationIdMiddleware.ResolveCorrelationId(StringValues.Empty, out var rejected);

        Assert.False(rejected);
        Assert.True(Guid.TryParseExact(resolved, "D", out _));
    }

    [Fact]
    public void Rejects_multiple_header_values()
    {
        CorrelationIdMiddleware.ResolveCorrelationId(new StringValues(["a", "b"]), out var rejected);
        Assert.True(rejected);
    }

    [Fact]
    public void Generated_ids_cannot_be_mistaken_for_w3c_trace_ids()
    {
        var generated = CorrelationIdMiddleware.ResolveCorrelationId(StringValues.Empty, out _);

        // W3C trace IDs are 32 lowercase hex characters without separators.
        Assert.Equal(36, generated.Length);
        Assert.Contains('-', generated);
    }
}
