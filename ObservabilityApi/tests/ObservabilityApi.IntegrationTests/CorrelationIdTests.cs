using System.Net;
using ObservabilityApi.IntegrationTests.Infrastructure;

namespace ObservabilityApi.IntegrationTests;

public sealed class CorrelationIdTests(ApiFactory factory)
{
    private readonly HttpClient client = factory.CreateClient();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Valid_incoming_correlation_id_is_echoed()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add(HttpAssertions.CorrelationHeader, "checkout-42");

        var response = await client.SendAsync(request, Ct);

        Assert.Equal("checkout-42", response.Headers.GetValues(HttpAssertions.CorrelationHeader).Single());
    }

    [Fact]
    public async Task Missing_correlation_id_is_generated()
    {
        var response = await client.GetAsync("/health/live", Ct);

        var value = response.Headers.GetValues(HttpAssertions.CorrelationHeader).Single();
        Assert.True(Guid.TryParseExact(value, "D", out _));
    }

    [Fact]
    public async Task Invalid_correlation_id_is_replaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.TryAddWithoutValidation(HttpAssertions.CorrelationHeader, new string('x', 200));

        var response = await client.SendAsync(request, Ct);

        var value = response.Headers.GetValues(HttpAssertions.CorrelationHeader).Single();
        Assert.True(Guid.TryParseExact(value, "D", out _));
    }

    [Fact]
    public async Task Correlation_id_is_kept_on_error_responses_and_is_distinct_from_trace_id()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{Guid.NewGuid()}");
        request.Headers.Add(HttpAssertions.CorrelationHeader, "support-ticket-7");

        var response = await client.SendAsync(request, Ct);

        var problem = await response.AssertProblemAsync(HttpStatusCode.NotFound);
        Assert.Equal("support-ticket-7", problem.GetProperty("correlationId").GetString());
        Assert.NotEqual(problem.GetProperty("traceId").GetString(), problem.GetProperty("correlationId").GetString());
    }

    [Fact]
    public async Task Incoming_w3c_traceparent_is_continued()
    {
        const string traceId = "4bf92f3577b34da6a3ce929d0e0e4736";
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{Guid.NewGuid()}");
        request.Headers.Add("traceparent", $"00-{traceId}-00f067aa0ba902b7-01");

        var response = await client.SendAsync(request, Ct);

        var problem = await response.AssertProblemAsync(HttpStatusCode.NotFound);
        Assert.Equal(traceId, problem.GetProperty("traceId").GetString());
    }
}
