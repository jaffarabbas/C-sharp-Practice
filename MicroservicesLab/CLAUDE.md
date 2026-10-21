# MicroservicesLab

Reference solution for **centralized configuration across ASP.NET Core microservices**. It is the working model for a refactor the user will repeat in their office project. Read `CONTEXT.md` for the why; the step-by-step procedure is the skill in `.claude/skills/centralize-microservice-config/SKILL.md`.

## Layout
```
config/appsettings.Shared*.json   the ONLY appsettings files (base, Development, Production)
Directory.Build.props             links config/appsettings.Shared*.json into every project's bin
Directory.Packages.props          central NuGet versions (projects have no Version attributes)
MicroservicesLab.slnLaunch        VS "All services" multi-start profile
src/BuildingBlocks                SharedConfigurationExtensions.AddSharedConfiguration()
src/Services/{Employee,Payroll,Leave}.Api   .NET 10 Web APIs (template endpoints)
src/Gateway/ApiGateway            YARP; routes built in code from config
src/Web/WebPortal                 Razor Pages; calls services via the gateway
src/Aspire/AppHost                OPTIONAL second way to run everything (dashboard, telemetry)
src/Aspire/ServiceDefaults        AddServiceDefaults(): OpenTelemetry, /health + /alive, HttpClient resilience
```

Two ways to run, both must keep working:
- **Without Aspire** (`.slnLaunch` "All services", or `dotnet run` per project): each app binds its port from `Services:<ProjectName>`.
- **With Aspire** (`.slnLaunch` "Aspire", or `dotnet run --project src/Aspire/AppHost`): AppHost reads the same shared config, puts its proxy on those same ports, and sets `Hosting__Orchestrated=true` so `AddSharedConfiguration()` does not bind the port itself.

## Rules
- Do not add `appsettings*.json` to a project. Shared values go in `config/`. Add a per-project file only for a key that genuinely differs for that one app, and keep it to that key.
- Ports live in `Services:<ProjectName>` in `config/appsettings.Shared.Development.json`, never in `launchSettings.json`. `<ProjectName>` must equal the assembly name (`builder.Environment.ApplicationName`).
- Every `Properties/launchSettings.json` is identical (one `Development` profile, no `applicationUrl`).
- Every app's `Program.cs` calls `builder.AddSharedConfiguration();` immediately after `CreateBuilder`.
- New package: add `<PackageVersion>` to `Directory.Packages.props`, then `<PackageReference Include="..."/>` without a version.
- New service: add the project + `BuildingBlocks` and `ServiceDefaults` references, call `AddSharedConfiguration()`, `AddServiceDefaults()` and `app.MapDefaultEndpoints()`, copy a `launchSettings.json`, add a `Services` entry (Development and Production), add a `Gateway:Routes` entry if it is exposed, add it to `.slnLaunch` "All services", and add an `AddService<...>` line in `src/Aspire/AppHost/AppHost.cs` (AppHost needs a project reference to it).
- Aspire template packages (13.6.2) are installed per user: `dotnet new install Aspire.ProjectTemplates::13.6.2`.

## Commands
```
dotnet build MicroservicesLab.slnx
dotnet run --project src/Services/Employee.Api      # any app; port comes from shared config
```
Verify end to end: start all five, then `GET http://localhost:5000/{employee|payroll|leave}/weatherforecast` → 200, and `http://localhost:5200` lists all three as `200 OK`.
