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

        // On a server, SHARED_CONFIG_DIR points every app at one folder (e.g. D:\AppConfig), so a setting is edited
        // once there instead of in each app's copy. It overrides the bundled copies; app-specific files still win.
        var serverConfigDir = builder.Configuration["SHARED_CONFIG_DIR"];
        if (!string.IsNullOrWhiteSpace(serverConfigDir))
        {
            if (!Directory.Exists(serverConfigDir))
                throw new DirectoryNotFoundException($"SHARED_CONFIG_DIR is set to '{serverConfigDir}', which does not exist.");

            var serverFiles = new PhysicalFileProvider(serverConfigDir);
            builder.Configuration.Sources.Insert(2, Json(serverFiles, "appsettings.Shared.json", optional: true));
            builder.Configuration.Sources.Insert(3, Json(serverFiles, $"appsettings.Shared.{environment}.json", optional: true));
        }

        // In Development the port comes from Services:<ProjectName> instead of launchSettings.json.
        // It deliberately wins over ASPNETCORE_URLS: Visual Studio sets that to https://localhost:5001;http://localhost:5000
        // when a profile has no applicationUrl, which would put every service on the same port.
        // Docker sets ASPNETCORE_HTTP_PORTS instead, so that one is still respected.
        // Outside Development these entries are addresses other services call (e.g. http://employee-api:8080),
        // so the host decides the binding. Under the Aspire AppHost (Hosting:Orchestrated) Aspire assigns the port.
        var url = builder.Configuration[$"Services:{builder.Environment.ApplicationName}"];
        var runningInContainer = !string.IsNullOrEmpty(builder.Configuration["http_ports"]);
        var orchestrated = string.Equals(builder.Configuration["Hosting:Orchestrated"], "true", StringComparison.OrdinalIgnoreCase);

        if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(url) && !runningInContainer && !orchestrated)
            builder.WebHost.UseUrls(url);

        return builder;
    }

    private static JsonConfigurationSource Json(IFileProvider files, string path, bool optional) =>
        new() { FileProvider = files, Path = path, Optional = optional, ReloadOnChange = true };
}
