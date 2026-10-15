using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ObservabilityApi.Infrastructure;

namespace ObservabilityApi.Api.Health;

internal static class HealthEndpoints
{
    /// <summary>
    /// /health/live: process is up and serving (no dependency checks, so a DB outage never triggers restarts).
    /// /health/ready: can serve traffic (SQL Server reachable). Seq and the Collector are intentionally not
    /// checked: losing telemetry must not take the API out of the load balancer.
    /// </summary>
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = WriteJsonAsync,
        }).DisableRateLimiting().DisableHttpMetrics();

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(DependencyInjection.ReadinessTag),
            ResponseWriter = WriteJsonAsync,
        }).DisableRateLimiting().DisableHttpMetrics();

        return endpoints;
    }

    private static Task WriteJsonAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        // Only names, statuses and generic descriptions; exception details stay in the logs.
        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 1),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                durationMs = Math.Round(e.Value.Duration.TotalMilliseconds, 1),
                description = e.Value.Description,
            }),
        };
        return JsonSerializer.SerializeAsync(context.Response.Body, payload, cancellationToken: context.RequestAborted);
    }
}
