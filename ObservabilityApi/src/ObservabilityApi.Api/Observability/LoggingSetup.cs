using System.Globalization;
using Microsoft.AspNetCore.Routing;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace ObservabilityApi.Api.Observability;

internal static class LoggingSetup
{
    private const string TextTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j} trace={TraceId} span={SpanId}{NewLine}{Exception}";

    /// <summary>
    /// First-stage logger used until the host is built, so configuration/DI failures are still logged.
    /// Replaced (not duplicated) by <see cref="AddSerilogLogging"/> once configuration is available.
    /// </summary>
    public static void CreateBootstrapLogger() =>
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();

    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder, ObservabilityOptions options)
    {
        builder.Services.AddSerilog((services, logger) =>
        {
            // Minimum levels and per-namespace overrides come from the "Serilog" configuration section.
            logger.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", options.ServiceName)
                .Enrich.WithProperty("ApplicationVersion", AppInfo.Version)
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .Enrich.WithProperty("MachineName", Environment.MachineName);

            // Serilog 4 captures Activity.Current's TraceId/SpanId on every event natively. The Seq sink ships
            // them as @tr/@sp, so Seq links log events to traces; the text template prints them explicitly.
            if (options.Console.Json)
            {
                logger.WriteTo.Console(new RenderedCompactJsonFormatter());
            }
            else
            {
                logger.WriteTo.Console(outputTemplate: TextTemplate, formatProvider: CultureInfo.InvariantCulture);
            }

            var seq = options.Seq;
            if (!string.IsNullOrWhiteSpace(seq.ServerUrl))
            {
                // Non-blocking: events are appended to a bounded in-memory queue and shipped in batches on a
                // background task. If Seq is down the sink retries with backoff, and once QueueSizeLimit is
                // reached new events are dropped rather than blocking request threads.
                logger.WriteTo.Seq(
                    serverUrl: seq.ServerUrl,
                    apiKey: string.IsNullOrWhiteSpace(seq.ApiKey) ? null : seq.ApiKey,
                    batchPostingLimit: seq.BatchPostingLimit,
                    period: TimeSpan.FromSeconds(seq.PeriodSeconds),
                    bufferBaseFilename: string.IsNullOrWhiteSpace(seq.BufferBaseFilename) ? null : seq.BufferBaseFilename,
                    queueSizeLimit: seq.QueueSizeLimit,
                    formatProvider: CultureInfo.InvariantCulture);
            }
        });

        return builder;
    }

    /// <summary>
    /// One structured completion event per request (replaces the chatty Microsoft.AspNetCore.Hosting logs).
    /// Query strings, headers and bodies are not logged.
    /// </summary>
    public static IApplicationBuilder UseRequestCompletionLogging(this IApplicationBuilder app, ObservabilityOptions options) =>
        app.UseSerilogRequestLogging(o =>
        {
            o.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            o.IncludeQueryInRequestPath = false;
            o.GetLevel = (context, elapsedMs, exception) =>
            {
                if (exception is not null || context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                {
                    return LogEventLevel.Error;
                }

                if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
                {
                    return LogEventLevel.Verbose; // Probes run every few seconds; keep them out of Seq.
                }

                return elapsedMs > options.SlowRequestThresholdMs ? LogEventLevel.Warning : LogEventLevel.Information;
            };
            o.EnrichDiagnosticContext = (diagnostics, context) =>
            {
                // The route template (e.g. api/products/{id:guid}) groups requests without per-ID values.
                if (context.GetEndpoint() is RouteEndpoint endpoint)
                {
                    diagnostics.Set("RouteTemplate", endpoint.RoutePattern.RawText);
                }
            };
        });
}
