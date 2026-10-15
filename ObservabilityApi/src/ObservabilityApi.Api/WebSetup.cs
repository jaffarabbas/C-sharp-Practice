using System.Diagnostics;
using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using ObservabilityApi.Api.Correlation;
using ObservabilityApi.Api.ErrorHandling;
using ObservabilityApi.Api.Health;
using ObservabilityApi.Api.Observability;
using Scalar.AspNetCore;

namespace ObservabilityApi.Api;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public bool Enabled { get; init; } = true;

    /// <summary>Requests allowed per client IP per window, per API instance (the limiter is in-memory).</summary>
    public int PermitLimit { get; init; } = 100;

    public int WindowSeconds { get; init; } = 10;
}

public sealed class RequestTimeoutOptions
{
    public const string SectionName = "RequestTimeouts";

    public int DefaultTimeoutSeconds { get; init; } = 30;
}

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    /// <summary>Exact origins allowed to call the API from browsers. Empty = no cross-origin access.</summary>
    public string[] AllowedOrigins { get; init; } = [];
}

internal static class WebSetup
{
    public static WebApplicationBuilder AddWebApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddControllers();
        services.AddOpenApi();

        // One ProblemDetails shape for every error source: exceptions, [ApiController] 400s,
        // NotFound(), status-code-only responses, rate limiting and timeouts.
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var http = context.HttpContext;
            context.ProblemDetails.Instance ??= $"{http.Request.Method} {http.Request.Path}";
            // Plain 32-hex W3C trace ID (the framework default is the full traceparent), searchable in Seq and Jaeger.
            context.ProblemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToHexString() ?? http.TraceIdentifier;
            context.ProblemDetails.Extensions["correlationId"] = CorrelationIdMiddleware.GetCorrelationId(http);
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();

        var rateLimiting = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>() ?? new();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                !rateLimiting.Enabled
                    ? RateLimitPartition.GetNoLimiter("disabled")
                    // Behind a proxy, enable ASPNETCORE_FORWARDEDHEADERS_ENABLED so RemoteIpAddress is the client.
                    : RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimiting.PermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimiting.WindowSeconds),
                            QueueLimit = 0,
                        }));
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
                }

                var problemDetails = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                await problemDetails.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails = { Title = "Too many requests.", Detail = "Rate limit exceeded. Retry after the period in the Retry-After header." },
                });
            };
        });

        var timeouts = configuration.GetSection(RequestTimeoutOptions.SectionName).Get<RequestTimeoutOptions>() ?? new();
        services.AddRequestTimeouts(options => options.DefaultPolicy = new RequestTimeoutPolicy
        {
            // On timeout the request's CancellationToken fires (aborting in-flight SQL) and a 504 is returned,
            // which UseStatusCodePages renders as ProblemDetails. Timeouts are not enforced under a debugger.
            Timeout = TimeSpan.FromSeconds(timeouts.DefaultTimeoutSeconds),
        });

        var cors = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new();
        if (builder.Environment.IsProduction() && cors.AllowedOrigins.Any(o => o.Contains('*', StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Wildcard CORS origins are not allowed in Production.");
        }

        services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            if (cors.AllowedOrigins.Length > 0)
            {
                policy.WithOrigins(cors.AllowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization", CorrelationIdMiddleware.HeaderName, "traceparent", "tracestate")
                    .WithExposedHeaders(CorrelationIdMiddleware.HeaderName, "Location", "Retry-After");
            }
        }));

        return builder;
    }

    public static WebApplication UseWebApi(this WebApplication app, ObservabilityOptions observability)
    {
        // Order matters:
        // 1. Correlation ID first, so every log event (including the request completion event) carries it.
        // 2. Request logging outside the exception handler: it sees the final status code, while the
        //    exception itself is logged once by GlobalExceptionHandler.
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseRequestCompletionLogging(observability);
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        if (app.Configuration.GetValue("Https:RedirectToHttps", false))
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseCors();
        app.UseRateLimiter();
        app.UseRequestTimeouts();

        app.MapHealthEndpoints();
        app.MapControllers();

        if (app.Configuration.GetValue("OpenApi:Enabled", app.Environment.IsDevelopment()))
        {
            app.MapOpenApi().DisableRateLimiting();
            app.MapScalarApiReference().DisableRateLimiting();
        }

        return app;
    }
}
