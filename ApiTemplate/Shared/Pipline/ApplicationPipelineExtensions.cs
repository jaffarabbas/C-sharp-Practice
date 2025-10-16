using ApiTemplate.Hubs;
using ApiTemplate.Middleware;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ApiTemplate.Services
{
    /// <summary>
    /// Central place to apply the HTTP request pipeline & custom middlewares.
    /// Add or reorder middleware here instead of editing Program.cs.
    /// </summary>
    public static class ApplicationPipelineExtensions
    {
        /// <summary>
        /// Applies the application's middleware pipeline (current order preserved).
        /// </summary>
        public static WebApplication UseApplicationPipeline(this WebApplication app)
        {

            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            // Add Serilog request logging (enriches logs with HTTP context)
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());

                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                        diagnosticContext.Set("UserId", httpContext.User.FindFirst("userId")?.Value);
                    }
                };
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                    {
                        // Configure Swagger UI for each API version
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                $"API {description.GroupName.ToUpperInvariant()}"
                            );
                        }

                        // Optional: Set default version
                        options.DefaultModelsExpandDepth(-1); // Hide models section
                        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                        options.DisplayRequestDuration();
                    });
            }

            // Performance monitoring middleware (should be early in pipeline)
            app.UseMiddleware<PerformanceMonitoringMiddleware>();

            // Request/Response logging middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // CORS
            app.UseCors(CorsPolicies.Default);
            app.UseHttpsRedirection();
            // Custom response wrapper
            app.UseApiResponse();
            // Custom middlewares
            app.UseExceptionMiddleware();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAuthMiddleware();
            app.UseRateLimiter();
            app.MapControllers();
            // SignalR hubs
            app.MapHub<ItemNotificationHub>("/itemNotificationHub");
            // Ensure Serilog is properly closed on shutdown
            // app.EnsureSerilogClosed();
            return app;
        }
    }
}