using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ObservabilityApi.Infrastructure.Persistence;

namespace ObservabilityApi.Infrastructure.Health;

/// <summary>
/// Readiness probe for SQL Server. Opens its own short-timeout connection instead of going through
/// EF Core, because the EF execution strategy would retry for tens of seconds and make the probe hang.
/// Only the database gates readiness; logging/tracing backends (Seq, Collector) intentionally do not.
/// </summary>
internal sealed class SqlServerReadinessCheck(AppDbContext db) : IHealthCheck
{
    private const int ConnectTimeoutSeconds = 3;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
            {
                ConnectTimeout = ConnectTimeoutSeconds,
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = ConnectTimeoutSeconds;
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException or OperationCanceledException)
        {
            // The description is returned to callers; keep it generic. The exception (which never contains
            // the password) is attached for the health check log entry.
            return new HealthCheckResult(context.Registration.FailureStatus, "SQL Server is not reachable.", ex);
        }
    }
}
