using System.Reflection;

namespace ObservabilityApi.Api.Observability;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    /// <summary>Logical service name: Serilog "Application" property and OpenTelemetry service.name.</summary>
    public string ServiceName { get; init; } = "observability-api";

    /// <summary>Requests slower than this are logged at Warning in the request completion event.</summary>
    public int SlowRequestThresholdMs { get; init; } = 1000;

    public ConsoleLogOptions Console { get; init; } = new();

    public SeqOptions Seq { get; init; } = new();

    public OtlpOptions Otlp { get; init; } = new();

    public TracingOptions Tracing { get; init; } = new();

    public MetricsOptions Metrics { get; init; } = new();
}

public sealed class ConsoleLogOptions
{
    /// <summary>Compact JSON (CLEF) for log collectors in production; human-readable text otherwise.</summary>
    public bool Json { get; init; }
}

public sealed class SeqOptions
{
    /// <summary>Seq ingestion URL. Empty disables the Seq sink.</summary>
    public string? ServerUrl { get; init; }

    /// <summary>Seq API key. Supply through user secrets or environment variables, never appsettings.</summary>
    public string? ApiKey { get; init; }

    /// <summary>Upper bound of events held in memory while Seq is slow or down; newer events are dropped beyond it.</summary>
    public int QueueSizeLimit { get; init; } = 100_000;

    public int BatchPostingLimit { get; init; } = 1_000;

    public double PeriodSeconds { get; init; } = 2;

    /// <summary>
    /// Optional path prefix for durable (disk-buffered) shipping. Survives restarts and longer outages
    /// at the cost of disk I/O and local state; leave empty for the default in-memory queue.
    /// </summary>
    public string? BufferBaseFilename { get; init; }
}

public sealed class OtlpOptions
{
    /// <summary>OpenTelemetry Collector OTLP endpoint (gRPC, e.g. http://localhost:4317). Empty disables export.</summary>
    public string? Endpoint { get; init; }
}

public sealed class TracingOptions
{
    /// <summary>
    /// Head sampling ratio for new (root) traces, 0.0-1.0. Parent-based: if an upstream service already made
    /// a sampling decision, it is honored so traces are never broken in the middle.
    /// </summary>
    public double SamplingRatio { get; init; } = 1.0;

    /// <summary>
    /// Export SQL statement text (db.query.text) on database spans. Off by default because statements can
    /// reveal schema and, for non-parameterized SQL, literal values. Parameter values are never exported.
    /// </summary>
    public bool CaptureDbStatements { get; init; }
}

public sealed class MetricsOptions
{
    public int ExportIntervalSeconds { get; init; } = 15;
}

internal static class AppInfo
{
    public static string Version { get; } =
        typeof(AppInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(AppInfo).Assembly.GetName().Version?.ToString()
        ?? "0.0.0";
}
