using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ObservabilityApi.Application.Products;

namespace ObservabilityApi.IntegrationTests.Infrastructure;

internal static class HttpAssertions
{
    public const string CorrelationHeader = "X-Correlation-ID";

    public static async Task<JsonElement> AssertProblemAsync(this HttpResponseMessage response, HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal((int)expectedStatus, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));

        // Every problem carries a plain W3C trace ID and the request's correlation ID.
        var traceId = problem.GetProperty("traceId").GetString();
        Assert.Matches("^[0-9a-f]{32}$", traceId);
        Assert.Equal(response.Headers.GetValues(CorrelationHeader).Single(), problem.GetProperty("correlationId").GetString());
        return problem;
    }

    public static async Task<ProductResponse> CreateProductAsync(this HttpClient client, string? sku = null, decimal price = 10m)
    {
        var response = await client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest(sku ?? UniqueSku(), "Test product", "Created by integration tests", price),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken))!;
    }

    public static string UniqueSku() => "IT-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
}
