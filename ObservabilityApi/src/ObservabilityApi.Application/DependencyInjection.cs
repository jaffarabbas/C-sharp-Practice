using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ObservabilityApi.Application.Products;
using ObservabilityApi.Application.Telemetry;

namespace ObservabilityApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Validators are stateless, so a singleton lifetime avoids per-request allocations.
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>(
            ServiceLifetime.Singleton, includeInternalTypes: false);
        services.AddScoped<ProductService>();
        services.AddSingleton<ProductMetrics>();
        services.TryAddSingleton(TimeProvider.System);
        return services;
    }
}
