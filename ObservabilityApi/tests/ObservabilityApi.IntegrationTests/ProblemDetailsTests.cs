using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ObservabilityApi.Application.Products;
using ObservabilityApi.IntegrationTests.Infrastructure;

namespace ObservabilityApi.IntegrationTests;

public sealed class ProblemDetailsTests(ApiFactory factory)
{
    private readonly HttpClient client = factory.CreateClient();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Validation_failure_returns_400_with_camel_case_field_errors()
    {
        var response = await client.PostAsJsonAsync("/api/products", new CreateProductRequest("!", "", null, -1m), Ct);

        var problem = await response.AssertProblemAsync(HttpStatusCode.BadRequest);
        var errors = problem.GetProperty("errors");
        Assert.True(errors.TryGetProperty("sku", out _));
        Assert.True(errors.TryGetProperty("name", out _));
        Assert.True(errors.TryGetProperty("price", out _));
        Assert.Equal("POST /api/products", problem.GetProperty("instance").GetString());
    }

    [Fact]
    public async Task Malformed_json_returns_400_problem()
    {
        using var content = new StringContent("{ \"sku\": ", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/products", content, Ct);

        await response.AssertProblemAsync(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Unknown_route_returns_404_problem()
    {
        var response = await client.GetAsync("/api/does-not-exist", Ct);
        await response.AssertProblemAsync(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Invalid_route_value_returns_404_problem()
    {
        var response = await client.GetAsync("/api/products/not-a-guid", Ct);
        await response.AssertProblemAsync(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unexpected_exception_returns_generic_500_without_internal_details()
    {
        // A second host whose clock throws, to simulate a bug deep inside a use case.
        using var failingFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(new ThrowingTimeProvider())));
        using var failingClient = failingFactory.CreateClient();

        var response = await failingClient.PostAsJsonAsync("/api/products",
            new CreateProductRequest(HttpAssertions.UniqueSku(), "Boom", null, 1m), Ct);

        var problem = await response.AssertProblemAsync(HttpStatusCode.InternalServerError);
        var body = problem.GetRawText();
        Assert.DoesNotContain(ThrowingTimeProvider.SecretMessage, body, StringComparison.Ordinal);
        Assert.DoesNotContain("at ObservabilityApi", body, StringComparison.Ordinal); // no stack trace
    }

    private sealed class ThrowingTimeProvider : TimeProvider
    {
        public const string SecretMessage = "internal-detail-that-must-not-leak";

        public override DateTimeOffset GetUtcNow() => throw new InvalidOperationException(SecretMessage);
    }
}
