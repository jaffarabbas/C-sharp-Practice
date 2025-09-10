using ApiTemplate.Hubs;
using ApiTemplate.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            }
            // Swagger (kept outside so Program.cs can still control env gating if desired)
            // CORS
            app.UseCors(CorsPolicies.Default);

            app.UseHttpsRedirection();

            // Custom response wrapper
            app.UseApiResponse();

            // (Your current order) Authorization BEFORE custom auth/token middlewares
            app.UseAuthorization();

            // Map controllers early (kept as you had it, though typically placed later)
            app.MapControllers();

            // Custom middlewares
            app.UseAuthMiddleware();
            app.UseExceptionMiddleware();
            app.UseValidateTokenHandler();

            // SignalR hubs
            app.MapHub<ItemNotificationHub>("/itemNotificationHub");

            return app;
        }
    }
}