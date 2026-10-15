using System.Diagnostics;
using System.Text.RegularExpressions;
using Serilog.Context;

namespace ObservabilityApi.Api.Correlation;

/// <summary>
/// Correlation ID = a caller-supplied business/request identifier that may span many traces (retries,
/// async workflows, support tickets). It is intentionally independent of the W3C trace ID, which identifies
/// one distributed trace and is owned by OpenTelemetry. Both are logged; only the trace ID drives tracing.
/// </summary>
internal sealed partial class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";
    public const string LogPropertyName = "CorrelationId";
    private static readonly object ItemKey = new();

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName], out var rejected);
        if (rejected)
        {
            // Never log the rejected value: it is untrusted input (log injection, oversized payloads).
            LogInvalidCorrelationId(logger, HeaderName);
        }

        context.Items[ItemKey] = correlationId;

        // OnStarting survives the exception handler clearing the response, so errors carry the header too.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Span attribute (high cardinality is fine on spans; it is never used as a metric label).
        Activity.Current?.SetTag("correlation.id", correlationId);

        using (LogContext.PushProperty(LogPropertyName, correlationId))
        {
            await next(context);
        }
    }

    public static string? GetCorrelationId(HttpContext context) =>
        context.Items.TryGetValue(ItemKey, out var value) ? value as string : null;

    /// <summary>Accepts a single header value of 1-64 characters from [A-Za-z0-9._:-]; otherwise generates one.</summary>
    internal static string ResolveCorrelationId(Microsoft.Extensions.Primitives.StringValues headerValues, out bool rejected)
    {
        rejected = false;
        if (headerValues.Count == 1 && IsValid(headerValues[0]))
        {
            return headerValues[0]!;
        }

        rejected = headerValues.Count > 0;

        // Dashed GUID format so it is visually distinct from 32-hex-character W3C trace IDs.
        return Guid.NewGuid().ToString("D");
    }

    internal static bool IsValid(string? value) => value is not null && AllowedPattern().IsMatch(value);

    [GeneratedRegex("^[A-Za-z0-9._:-]{1,64}$", RegexOptions.CultureInvariant)]
    private static partial Regex AllowedPattern();

    [LoggerMessage(EventId = 2001, EventName = "InvalidCorrelationId", Level = LogLevel.Information,
        Message = "Ignored invalid {HeaderName} request header; generated a new correlation ID")]
    private static partial void LogInvalidCorrelationId(ILogger logger, string headerName);
}
