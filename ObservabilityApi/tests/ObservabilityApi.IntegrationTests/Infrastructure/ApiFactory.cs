using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ObservabilityApi.Infrastructure.Persistence;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(ObservabilityApi.IntegrationTests.Infrastructure.ApiFactory))]

namespace ObservabilityApi.IntegrationTests.Infrastructure;

/// <summary>
/// Hosts the real API against a disposable SQL Server container (Testcontainers). Tests never touch the
/// developer database: the "Testing" environment does not load user secrets, the container listens on a
/// random host port, and a guard below refuses to run against a database named test2.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Pinned tag; override with TEST_MSSQL_IMAGE (e.g. an internal registry mirror).
    private static readonly string Image =
        Environment.GetEnvironmentVariable("TEST_MSSQL_IMAGE") ?? "mcr.microsoft.com/mssql/server:2022-CU23-ubuntu-22.04";

    private const string TestDatabase = "ObservabilityApiTests";

    private readonly MsSqlContainer container = new MsSqlBuilder(Image).Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();

        var builder = new SqlConnectionStringBuilder(container.GetConnectionString()) { InitialCatalog = TestDatabase };
        if (string.Equals(builder.InitialCatalog, "test2", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Integration tests must never run against the test2 database.");
        }

        ConnectionString = builder.ConnectionString;

        // Apply the real migrations, exactly as production would.
        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Database", ConnectionString);
        // No telemetry backends in tests: sinks/exporters are disabled by empty endpoints.
        builder.UseSetting("Observability:Seq:ServerUrl", string.Empty);
        builder.UseSetting("Observability:Otlp:Endpoint", string.Empty);
        builder.UseSetting("RateLimiting:PermitLimit", "100000");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await container.DisposeAsync();
    }
}
