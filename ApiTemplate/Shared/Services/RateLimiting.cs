using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiTemplate.Shared.Configuration;

namespace ApiTemplate.Shared.Services
{
    public static class RateLimitingService
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration
            var rateLimitOptions = new RateLimitingOptions();
            configuration.GetSection(RateLimitingOptions.SectionName).Bind(rateLimitOptions);

            // Register options for DI
            services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

            services.AddRateLimiter(options =>
            {
                // Fixed Window Rate Limiter
                options.AddFixedWindowLimiter("FixedPolicy", configure =>
                {
                    var config = rateLimitOptions.FixedPolicy;
                    configure.PermitLimit = config.PermitLimit;
                    configure.Window = TimeSpan.FromMinutes(config.WindowMinutes);
                    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    configure.QueueLimit = config.QueueLimit;
                });

                // Sliding Window Rate Limiter
                options.AddSlidingWindowLimiter("SlidingPolicy", configure =>
                {
                    var config = rateLimitOptions.SlidingPolicy;
                    configure.PermitLimit = config.PermitLimit;
                    configure.Window = TimeSpan.FromMinutes(config.WindowMinutes);
                    configure.SegmentsPerWindow = config.SegmentsPerWindow;
                    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    configure.QueueLimit = config.QueueLimit;
                });

                // Token Bucket Rate Limiter
                options.AddTokenBucketLimiter("TokenPolicy", configure =>
                {
                    var config = rateLimitOptions.TokenPolicy;
                    configure.TokenLimit = config.TokenLimit;
                    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    configure.QueueLimit = config.QueueLimit;
                    configure.ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriodSeconds);
                    configure.TokensPerPeriod = config.TokensPerPeriod;
                    configure.AutoReplenishment = config.AutoReplenishment;
                });

                // Concurrency Limiter
                options.AddConcurrencyLimiter("ConcurrencyPolicy", configure =>
                {
                    var config = rateLimitOptions.ConcurrencyPolicy;
                    configure.PermitLimit = config.PermitLimit;
                    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    configure.QueueLimit = config.QueueLimit;
                });

                // Per IP Policy
                options.AddPolicy("PerIPPolicy", context =>
                {
                    var config = rateLimitOptions.PerIPPolicy;
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = config.AutoReplenishment,
                            PermitLimit = config.PermitLimit,
                            Window = TimeSpan.FromMinutes(config.WindowMinutes)
                        });
                });

                // Per User Policy
                options.AddPolicy("PerUserPolicy", context =>
                {
                    var config = rateLimitOptions.PerUserPolicy;
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? "Anonymous",
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = config.AutoReplenishment,
                            PermitLimit = config.PermitLimit,
                            Window = TimeSpan.FromMinutes(config.WindowMinutes),
                            SegmentsPerWindow = config.SegmentsPerWindow
                        });
                });

                // Tiered Policy
                options.AddPolicy("TieredPolicy", context =>
                {
                    var config = rateLimitOptions.TieredPolicy;
                    var userRole = context.User.FindFirst("role")?.Value;

                    return userRole switch
                    {
                        "Premium" => RateLimitPartition.GetTokenBucketLimiter("Premium",
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = config.Premium.TokenLimit,
                                ReplenishmentPeriod = TimeSpan.FromMinutes(config.Premium.ReplenishmentPeriodMinutes),
                                TokensPerPeriod = config.Premium.TokensPerPeriod
                            }),
                        "Basic" => RateLimitPartition.GetTokenBucketLimiter("Basic",
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = config.Basic.TokenLimit,
                                ReplenishmentPeriod = TimeSpan.FromMinutes(config.Basic.ReplenishmentPeriodMinutes),
                                TokensPerPeriod = config.Basic.TokensPerPeriod
                            }),
                        _ => RateLimitPartition.GetTokenBucketLimiter("Anonymous",
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = config.Anonymous.TokenLimit,
                                ReplenishmentPeriod = TimeSpan.FromMinutes(config.Anonymous.ReplenishmentPeriodMinutes),
                                TokensPerPeriod = config.Anonymous.TokensPerPeriod
                            })
                    };
                });

                // Global rate limiter
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var config = rateLimitOptions.GlobalLimiter;
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = config.AutoReplenishment,
                            PermitLimit = config.PermitLimit,
                            Window = TimeSpan.FromMinutes(config.WindowMinutes)
                        });
                });

                // Custom rejection response
                options.OnRejected = async (context, token) =>
                {
                    var config = rateLimitOptions.OnRejected;
                    context.HttpContext.Response.StatusCode = config.StatusCode;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsync(
                            string.Format(config.RetryAfterMessage, retryAfter.TotalMinutes),
                            cancellationToken: token);
                    }
                    else
                    {
                        await context.HttpContext.Response.WriteAsync(
                            config.Message,
                            cancellationToken: token);
                    }
                };
            });

            return services;
        }
    }
}