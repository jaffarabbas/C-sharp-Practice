using System.Diagnostics;
using ObservabilityApi.Application.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ObservabilityApi.Api.Observability;

/// <summary>
/// Traces and metrics go to the OpenTelemetry Collector over OTLP. Logs are deliberately NOT exported
/// through OpenTelemetry: Serilog ships them to Seq, and a second log pipeline would duplicate every event.
/// </summary>
internal static class TelemetrySetup
{
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder, ObservabilityOptions options)
    {
        // W3C trace context (traceparent/tracestate) + W3C baggage.
        Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(
        [
            new TraceContextPropagator(),
            new BaggagePropagator(),
        ]));

        var otlpEndpoint = string.IsNullOrWhiteSpace(options.Otlp.Endpoint) ? null : new Uri(options.Otlp.Endpoint);
        var seqUri = string.IsNullOrWhiteSpace(options.Seq.ServerUrl) ? null : new Uri(options.Seq.ServerUrl);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: options.ServiceName,
                    serviceVersion: AppInfo.Version,
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes(
                [
                    new("deployment.environment.name", builder.Environment.EnvironmentName.ToLowerInvariant()),
                ]))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(options.Tracing.SamplingRatio)))
                    .AddSource(ApplicationTelemetry.ActivitySourceName)
                    .AddAspNetCoreInstrumentation(aspnet =>
                    {
                        // Health probes would produce a trace every few seconds per instance.
                        aspnet.Filter = context => !context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
                    })
                    .AddHttpClientInstrumentation(http =>
                    {
                        // The Seq sink posts log batches over HttpClient; tracing those would create a
                        // root span every few seconds (and a feedback loop), so exclude them.
                        http.FilterHttpRequestMessage = request => !IsSameOrigin(request.RequestUri, seqUri);
                    })
                    .AddSqlClientInstrumentation(sql => sql.RecordException = true);

                if (!options.Tracing.CaptureDbStatements)
                {
                    // Must be registered before the exporter: processors run in registration order.
                    tracing.AddProcessor(new DbStatementRedactionProcessor());
                }

                if (otlpEndpoint is not null)
                {
                    // Batched, background export with a bounded queue (default 2048 spans): if the Collector is
                    // down, spans are dropped after export timeouts; request threads are never blocked.
                    tracing.AddOtlpExporter(otlp => otlp.Endpoint = otlpEndpoint);
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    // http.server.request.duration, active requests, Kestrel, routing, rate limiting, diagnostics.
                    .AddAspNetCoreInstrumentation()
                    // http.client.request.duration (outgoing calls, including Seq log shipping).
                    .AddHttpClientInstrumentation()
                    // db.client.operation.duration from the OpenTelemetry SqlClient instrumentation.
                    .AddSqlClientInstrumentation()
                    // Built-in .NET 9+ runtime meter: GC, JIT, thread pool, exceptions, process CPU and memory.
                    .AddMeter("System.Runtime")
                    .AddMeter(ApplicationTelemetry.MeterName)
                    .AddMeter([.. Infrastructure.DependencyInjection.MeterNames]);

                if (otlpEndpoint is not null)
                {
                    metrics.AddOtlpExporter((otlp, reader) =>
                    {
                        otlp.Endpoint = otlpEndpoint;
                        reader.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds =
                            options.Metrics.ExportIntervalSeconds * 1000;
                    });
                }
            });

        return builder;
    }

    private static bool IsSameOrigin(Uri? requestUri, Uri? other) =>
        requestUri is not null && other is not null
        && Uri.Compare(requestUri, other, UriComponents.SchemeAndServer, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;
}

/// <summary>
/// Removes SQL statement text from database spans. OpenTelemetry.Instrumentation.SqlClient always records
/// <c>db.query.text</c>; this keeps the low-cardinality <c>db.query.summary</c> (e.g. "SELECT catalog.Products")
/// and drops the full statement unless <see cref="TracingOptions.CaptureDbStatements"/> is enabled.
/// </summary>
internal sealed class DbStatementRedactionProcessor : BaseProcessor<Activity>
{
    private static readonly string[] StatementTags = ["db.query.text", "db.statement"];

    public override void OnEnd(Activity data)
    {
        foreach (var tag in StatementTags)
        {
            if (data.GetTagItem(tag) is not null)
            {
                data.SetTag(tag, null);
            }
        }
    }
}
