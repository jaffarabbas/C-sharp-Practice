using ApiTemplate.Hubs;
using ApiTemplate.Middleware;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            // Swagger (kept outside so Program.cs can still control env gating if desired)
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

            return app;
        }
    }
}