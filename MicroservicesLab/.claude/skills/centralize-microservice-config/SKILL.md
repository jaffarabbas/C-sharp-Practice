---
name: centralize-microservice-config
description: Move duplicated appsettings.json and launchSettings.json out of every ASP.NET Core microservice into one shared config folder, with ports, gateway routes and package versions defined once. Use when a .NET solution has several services/gateway/web apps carrying copies of the same settings, or when asked to "centralize appsettings", "manage launchSettings in one place", or apply the MicroservicesLab pattern to another solution.
---

# Centralize microservice configuration

End state: one `config/` folder holds every setting; each app has no appsettings files, an identical minimal `launchSettings.json`, and one extra line in `Program.cs`. Background and reasoning: `CONTEXT.md` next to the reference solution (MicroservicesLab).

Work on a branch. Never delete a settings file before its values are in the shared file and the app has been run.

## 1. Inspect before changing anything

1. List every runnable project (`Microsoft.NET.Sdk.Web`) and its assembly name (`<AssemblyName>` or the csproj file name). That name is the key used in `Services:`.
2. Note the .NET version and the hosting style in each `Program.cs`:
   - `WebApplication.CreateBuilder(args)` → use the main extension below.
   - `Host.CreateDefaultBuilder(args)` + `Startup` → use the `IHostBuilder` variant in step 4.
3. Diff all `appsettings.json` files against each other, then all `appsettings.Development.json`, etc. Report every key that differs between services and **ask the user** how to handle it before continuing. Do not assume "all the same" is literally true.
4. Collect each app's ports from `Properties/launchSettings.json` (`applicationUrl`) and any extra `environmentVariables`.
5. Find secrets (passwords, keys, tokens) in the JSON. These go to user-secrets / environment variables, not the shared file. Tell the user which ones.
6. Search for code that reads settings files directly instead of through `IConfiguration`: `appsettings.json`, `AddJsonFile`, `new ConfigurationBuilder()`, `File.ReadAllText(` on config files. Typical hits: EF Core `IDesignTimeDbContextFactory` (breaks `dotnet ef`), static config helpers, worker services built with `Host.CreateApplicationBuilder`/`Host.CreateDefaultBuilder` (need an `IHostApplicationBuilder`/`IHostBuilder` version of the extension). Each must also load `Path.Combine(AppContext.BaseDirectory, "appsettings.Shared.json")` before its own file, or switch to the host's `IConfiguration`. List them for the user.
7. Check for an existing `Directory.Build.props`, `Directory.Packages.props`, and the gateway type (YARP, Ocelot, custom). Merge into existing files rather than overwriting.

Show the user a short summary of findings and the plan; proceed after confirmation.

## 2. Shared config folder (solution root)

```
config/appsettings.Shared.json               keys common to all environments
config/appsettings.Shared.Development.json   dev DB connection, Services map with localhost ports
config/appsettings.Shared.Production.json    Services map with real internal addresses; no secrets
```

`Services` maps assembly name → base URL. Example (Development):

```json
{
  "ConnectionStrings": { "HrDb": "Server=localhost;Database=HrDb;Trusted_Connection=True;TrustServerCertificate=True" },
  "Services": {
    "ApiGateway":   "http://localhost:5000",
    "Employee.Api": "http://localhost:5101",
    "WebPortal":    "http://localhost:5200"
  }
}
```

Reuse the ports found in step 1.4 so bookmarks, Postman collections and front-end configs keep working.

## 3. `Directory.Build.props` (solution root)

```xml
<Project>
  <!-- One appsettings for every app: config/appsettings.Shared*.json is copied into each project's bin folder. -->
  <ItemGroup>
    <None Include="$(MSBuildThisFileDirectory)config\appsettings.Shared*.json"
          Link="%(Filename)%(Extension)"
          CopyToOutputDirectory="PreserveNewest"
          CopyToPublishDirectory="PreserveNewest" />
  </ItemGroup>
</Project>
```

If the file already exists, add only the `ItemGroup`. If `Directory.Build.props` files exist in subfolders, they hide the root one; add an `<Import>` of the parent there.

## 4. Shared library: `BuildingBlocks`

Create a class library (or reuse an existing shared library) with `<FrameworkReference Include="Microsoft.AspNetCore.App" />`, and reference it from every web project.

```csharp
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
        // It deliberately wins over ASPNETCORE_URLS: Visual Studio sets that to https://localhost:5001;http://localhost:5000
        // when a profile has no applicationUrl, which would put every service on the same port.
        // Docker sets ASPNETCORE_HTTP_PORTS instead, so that one is still respected.
        // Outside Development these entries are addresses other services call (e.g. http://employee-api:8080),
        // so the host decides the binding.
        var url = builder.Configuration[$"Services:{builder.Environment.ApplicationName}"];
        var runningInContainer = !string.IsNullOrEmpty(builder.Configuration["http_ports"]);

        if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(url) && !runningInContainer)
            builder.WebHost.UseUrls(url);

        return builder;
    }

    private static JsonConfigurationSource Json(IFileProvider files, string path, bool optional) =>
        new() { FileProvider = files, Path = path, Optional = optional, ReloadOnChange = true };
}
```

In each `Program.cs`, directly after `var builder = WebApplication.CreateBuilder(args);`:

```csharp
builder.AddSharedConfiguration();
```

**`IHostBuilder` / `Startup` variant (not tested in the lab; build and run it before relying on it):**

```csharp
public static IHostBuilder AddSharedConfiguration(this IHostBuilder host) =>
    host.ConfigureAppConfiguration((context, config) =>
    {
        var files = new PhysicalFileProvider(AppContext.BaseDirectory);
        var environment = context.HostingEnvironment.EnvironmentName;
        config.Sources.Insert(0, Json(files, "appsettings.Shared.json", optional: false));
        config.Sources.Insert(1, Json(files, $"appsettings.Shared.{environment}.json", optional: true));
    });
```

Call it as `Host.CreateDefaultBuilder(args).AddSharedConfiguration().ConfigureWebHostDefaults(...)`. This variant does not set ports. In this hosting style, either leave `applicationUrl` in that project's `launchSettings.json` (simplest), or move the project to minimal hosting first. Tell the user which one was done.

Why `AppContext.BaseDirectory` and `Insert(0)`: see CONTEXT.md. Do not change to `AddJsonFile` appended at the end; that makes the shared file override environment variables in production.

## 5. launchSettings.json

Replace every project's `Properties/launchSettings.json` with the same file:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "Development": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Keep a project-specific `environmentVariables` entry only if step 1.4 found one that cannot move to shared config. If the team uses IIS Express profiles, ask before removing them.

## 6. Remove per-service appsettings

Only after steps 2–5: delete each service's `appsettings*.json`, except keys the user decided (step 1.3) must stay service-specific. Those stay in a trimmed per-service `appsettings.json` and override the shared file automatically.

## 7. Gateway

- **YARP:** build routes from config so service URLs are not duplicated:

```csharp
var routeMap = builder.Configuration.GetSection("Gateway:Routes").GetChildren()
    .ToDictionary(r => r.Key, r => r.Value!);   // "employee": "Employee.Api"

var routes = routeMap.Select(r => new RouteConfig
{
    RouteId = r.Key,
    ClusterId = r.Value,
    Match = new RouteMatch { Path = $"/{r.Key}/{{**rest}}" }
}.WithTransformPathRemovePrefix($"/{r.Key}")).ToList();

var clusters = routeMap.Values.Distinct().Select(service => new ClusterConfig
{
    ClusterId = service,
    Destinations = new Dictionary<string, DestinationConfig>
    {
        ["default"] = new() { Address = builder.Configuration[$"Services:{service}"]! }
    }
}).ToList();

builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);
// ... app.MapReverseProxy();
```

- **Existing YARP JSON config or Ocelot:** do not rewrite it unasked. Move the file's content into the shared config (or keep it in the gateway project) and tell the user its addresses duplicate `Services:`.
- Web front end: register `AddHttpClient("gateway", c => c.BaseAddress = new Uri(config["Services:ApiGateway"]!.TrimEnd('/') + "/"))`.

## 8. Optional: central package versions

Create `Directory.Packages.props` with `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`, move every `Version="..."` from the csproj files into `<PackageVersion>` items, and remove the attributes. Requires NuGet 6.2+ (VS 2022 17.2+). If versions of the same package differ between projects, ask which to keep. Check `dotnet build` output for `NU1903`-style vulnerability warnings and pin patched versions here.

## 9. Optional: start everything

`<Solution>.slnLaunch` at the solution root (VS 2022 17.10+):

```json
[ { "Name": "All services", "Projects": [
    { "Path": "src\\Services\\Employee.Api\\Employee.Api.csproj", "Action": "Start" }
] } ]
```

## 9b. Optional: .NET Aspire as a second way to run (.NET 8+ only)

Only if the user asks. It must not replace the steps above; both ways of running must keep working.

1. `dotnet new install Aspire.ProjectTemplates`, then `dotnet new aspire-apphost` and `dotnet new aspire-servicedefaults`. With central package management, move the ServiceDefaults template's `Version` attributes into `Directory.Packages.props`.
2. Every app references ServiceDefaults and calls `builder.AddServiceDefaults();` (after `AddSharedConfiguration()`) and `app.MapDefaultEndpoints();`. Warn the user: this adds retries/timeouts to every `HttpClient` and `/health` + `/alive` in Development.
3. AppHost references every app and loads `appsettings.Shared.json` + `.{Env}.json` from `AppContext.BaseDirectory` (Insert at 0/1), so it shares the port map and connection string.
4. In `AddSharedConfiguration()`, skip `UseUrls` when `Hosting:Orchestrated` is `true`. Each project in AppHost:

```csharp
builder.AddProject<TProject>(name)
    .WithHttpEndpoint(port: new Uri(builder.Configuration[$"Services:{projectName}"]!).Port) // proxy on the usual port
    .WithEnvironment("Hosting__Orchestrated", "true")
    .WithReference(builder.AddConnectionString("HrDb"));   // create the connection-string resource once and reuse it
```

   `WithHttpEndpoint` is required because the launch profiles have no `applicationUrl`, so Aspire finds no endpoint otherwise. Add `.WithReference(x).WaitFor(x)` for gateway → services and web → gateway.
5. Add an "Aspire" entry (AppHost only) to the `.slnLaunch`.
6. Verify both ways: the five-app startup still binds the shared ports; AppHost alone serves the gateway and web on the same ports.

## 10. Verify

1. `dotnet build` with 0 errors; report any new warnings.
2. Start each app; its log shows `Now listening on:` with the port from the shared Development file.
3. Hit one endpoint per service directly and through the gateway.
4. Check one setting is really read, e.g. that the service connects to the database.
5. Report what was verified and what was not (Production, Docker, IIS are usually not testable locally).

Report results plainly, including failures. Do not commit unless the user asks.
