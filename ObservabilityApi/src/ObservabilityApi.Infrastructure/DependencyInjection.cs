using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ObservabilityApi.Application.Abstractions;
using ObservabilityApi.Infrastructure.Diagnostics;
using ObservabilityApi.Infrastructure.Health;
using ObservabilityApi.Infrastructure.Persistence;

namespace ObservabilityApi.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "Database";
    public const string ReadinessTag = "ready";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        // Never log or echo the connection string; only report that it is missing.
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'ConnectionStrings:{ConnectionStringName}' is not configured. " +
                "Set it with 'dotnet user-secrets' (development) or the ConnectionStrings__Database environment variable.");
        }

        services.AddDbContextPool<AppDbContext>(db => db
            .ConfigureWarnings(w => w.Log(
                // EF logs failed commands/SaveChanges at Error before the exception reaches the API boundary,
                // which then logs it again (500) or maps it to an expected 409. Keep one log per failure.
                (RelationalEventId.CommandError, LogLevel.Debug),
                (CoreEventId.SaveChangesFailed, LogLevel.Debug),
                (CoreEventId.QueryIterationFailed, LogLevel.Debug),
                // Transient-fault retries are an operational signal worth seeing at the default level.
                (CoreEventId.ExecutionStrategyRetrying, LogLevel.Warning)))
            .UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsHistoryTable("__EFMigrationsHistory", AppDbContext.Schema);
                sql.CommandTimeout(options.CommandTimeoutSeconds);

                // Retries transient faults (failover, throttling, dropped connections). Writes are safe to retry:
                // SaveChanges runs in a transaction and keys are generated client-side before the first attempt,
                // so a retry after an ambiguous commit hits the PK/unique index (409) instead of duplicating data.
                sql.EnableRetryOnFailure(
                    maxRetryCount: options.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(options.MaxRetryDelaySeconds),
                    errorNumbersToAdd: null);
            }));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddHealthChecks()
            .AddCheck<SqlServerReadinessCheck>(
                "sqlserver",
                failureStatus: HealthStatus.Unhealthy,
                tags: [ReadinessTag],
                timeout: TimeSpan.FromSeconds(5));

        services.AddHostedService<SqlClientPoolMetrics>();
        return services;
    }

    /// <summary>Meter names published by Infrastructure, for the OpenTelemetry registration in the Api.</summary>
    public static IReadOnlyList<string> MeterNames { get; } =
    [
        SqlClientPoolMetrics.MeterName,
        "Microsoft.EntityFrameworkCore",
    ];
}

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public int CommandTimeoutSeconds { get; init; } = 30;

    public int MaxRetryCount { get; init; } = 5;

    public int MaxRetryDelaySeconds { get; init; } = 10;
}
