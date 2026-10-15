using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ObservabilityApi.IntegrationTests.Infrastructure;

namespace ObservabilityApi.IntegrationTests;

public sealed class HealthTests(ApiFactory factory)
{
    private readonly HttpClient client = factory.CreateClient();
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Liveness_does_not_check_dependencies()
    {
        var response = await client.GetAsync("/health/live", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Ct);
        Assert.Equal("Healthy", body.GetProperty("status").GetString());
        Assert.Equal(0, body.GetProperty("checks").GetArrayLength());
    }

    [Fact]
    public async Task Readiness_checks_sql_server_only()
    {
        var response = await client.GetAsync("/health/ready", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Ct);
        var check = Assert.Single(body.GetProperty("checks").EnumerateArray());
        Assert.Equal("sqlserver", check.GetProperty("name").GetString());
        Assert.Equal("Healthy", check.GetProperty("status").GetString());
    }
}
