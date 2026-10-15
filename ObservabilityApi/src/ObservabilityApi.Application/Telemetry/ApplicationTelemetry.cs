using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ObservabilityApi.Application.Telemetry;

/// <summary>
/// Names of the application's own telemetry sources. The Api layer subscribes to these names;
/// the application layer only depends on System.Diagnostics (no OpenTelemetry SDK reference).
/// </summary>
public static class ApplicationTelemetry
{
    public const string ActivitySourceName = "ObservabilityApi.Application";
    public const string MeterName = "ObservabilityApi.Products";

    public static readonly ActivitySource ActivitySource = new(
        ActivitySourceName,
        typeof(ApplicationTelemetry).Assembly.GetName().Version?.ToString());
}

/// <summary>
/// Business metrics. Instruments carry no per-entity tags (no product IDs or SKUs):
/// every tag value must come from a small, fixed set to keep series cardinality bounded.
/// </summary>
public sealed class ProductMetrics
{
    private readonly Counter<long> created;
    private readonly Counter<long> updated;
    private readonly Counter<long> deleted;

    public ProductMetrics(IMeterFactory meterFactory)
    {
        ArgumentNullException.ThrowIfNull(meterFactory);
        var meter = meterFactory.Create(ApplicationTelemetry.MeterName);
        created = meter.CreateCounter<long>("products.created", "{product}", "Products successfully created.");
        updated = meter.CreateCounter<long>("products.updated", "{product}", "Products successfully updated.");
        deleted = meter.CreateCounter<long>("products.deleted", "{product}", "Products successfully deleted.");
    }

    public void ProductCreated() => created.Add(1);

    public void ProductUpdated() => updated.Add(1);

    public void ProductDeleted() => deleted.Add(1);
}
