using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Hosting;

namespace ObservabilityApi.Infrastructure.Diagnostics;

/// <summary>
/// Bridges Microsoft.Data.SqlClient connection-pool <b>EventCounters</b> into System.Diagnostics.Metrics
/// gauges so OpenTelemetry can export them. SqlClient 6.x does not publish pool metrics as Meters, and the
/// OpenTelemetry EventCounters instrumentation package is still pre-release, hence this small bridge.
/// <para>
/// Source: EventSource "Microsoft.Data.SqlClient.EventSource". Values are process-wide (all pools),
/// sampled every <see cref="SampleIntervalSeconds"/> seconds, and exported under meter
/// <see cref="MeterName"/> with the <c>sqlclient.pool.</c> prefix.
/// </para>
/// </summary>
internal sealed class SqlClientPoolMetrics : EventListener, IHostedService
{
    public const string MeterName = "ObservabilityApi.SqlClient.Pool";
    private const string SqlClientEventSourceName = "Microsoft.Data.SqlClient.EventSource";
    private const int SampleIntervalSeconds = 10;

    // Field initializers run before the EventListener base constructor, which may already call
    // OnEventSourceCreated for existing sources.
    private readonly ConcurrentDictionary<string, double> latest = new(StringComparer.Ordinal);

    private static readonly (string Counter, string Instrument, string Description)[] Gauges =
    [
        ("active-hard-connections", "sqlclient.pool.active_hard_connections", "Physical connections currently open to SQL Server."),
        ("number-of-pooled-connections", "sqlclient.pool.pooled_connections", "Connections managed by the connection pool."),
        ("number-of-active-connections", "sqlclient.pool.active_connections", "Pooled connections currently in use by the application."),
        ("number-of-free-connections", "sqlclient.pool.free_connections", "Pooled connections available for use."),
        ("number-of-stasis-connections", "sqlclient.pool.stasis_connections", "Connections waiting for an action to complete, unavailable to the application."),
        ("number-of-active-connection-pools", "sqlclient.pool.active_pools", "Active connection pools."),
    ];

    public SqlClientPoolMetrics(IMeterFactory meterFactory)
    {
        ArgumentNullException.ThrowIfNull(meterFactory);
        var meter = meterFactory.Create(MeterName);
        foreach (var (counter, instrument, description) in Gauges)
        {
            meter.CreateObservableGauge(instrument, () => latest.GetValueOrDefault(counter), "{connection}", description);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // The DI container disposes the listener (EventListener is IDisposable) on shutdown.
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name != SqlClientEventSourceName)
        {
            return;
        }

        // Critical level keeps SqlClient's verbose trace events disabled; counters are enabled by the
        // interval argument alone.
        EnableEvents(eventSource, EventLevel.Critical, EventKeywords.None, new Dictionary<string, string?>
        {
            ["EventCounterIntervalSec"] = SampleIntervalSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture),
        });
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName != "EventCounters" || eventData.Payload is not { Count: > 0 } payload
            || payload[0] is not IDictionary<string, object?> counter
            || counter.TryGetValue("Name", out var name) is false || name is not string counterName)
        {
            return;
        }

        // PollingCounter publishes "Mean"; ignore rate counters, which publish "Increment".
        if (counter.TryGetValue("Mean", out var mean) && mean is double value)
        {
            latest[counterName] = value;
        }
    }
}
