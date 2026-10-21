using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks;

public static class SharedConfigurationExtensions
{
    // Loads appsettings.Shared.json + appsettings.Shared.{Environment}.json as the lowest-priority sources,
    // so environment variables, user-secrets and command-line args can still override them.
    public static WebApplicationBuilder AddSharedConfiguration(this WebApplicationBuilder builder)
    {
        var files = new PhysicalFileProvider(AppContext.BaseDirectory);
        var environment = builder.Environment.EnvironmentName;

        builder.Configuration.Sources.Insert(0, Json(files, "appsettings.Shared.json", optional: false));
        builder.Configuration.Sources.Insert(1, Json(files, $"appsettings.Shared.{environment}.json", optional: true));

        // In Development the port comes from Services:<ProjectName> instead of launchSettings.json.
        // Elsewhere those entries are addresses other services call (e.g. http://employee-api:8080),
        // so the host decides the binding (ASPNETCORE_URLS / ASPNETCORE_HTTP_PORTS / IIS).
        var url = builder.Configuration[$"Services:{builder.Environment.ApplicationName}"];
        var hostHasUrl = !string.IsNullOrEmpty(builder.Configuration["urls"])
                         || !string.IsNullOrEmpty(builder.Configuration["http_ports"]);

        if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(url) && !hostHasUrl)
            builder.WebHost.UseUrls(url);

        return builder;
    }

    private static JsonConfigurationSource Json(IFileProvider files, string path, bool optional) =>
        new() { FileProvider = files, Path = path, Optional = optional, ReloadOnChange = true };
}
