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
```

## Rules
- Do not add `appsettings*.json` to a project. Shared values go in `config/`. Add a per-project file only for a key that genuinely differs for that one app, and keep it to that key.
- Ports live in `Services:<ProjectName>` in `config/appsettings.Shared.Development.json`, never in `launchSettings.json`. `<ProjectName>` must equal the assembly name (`builder.Environment.ApplicationName`).
- Every `Properties/launchSettings.json` is identical (one `Development` profile, no `applicationUrl`).
- Every app's `Program.cs` calls `builder.AddSharedConfiguration();` immediately after `CreateBuilder`.
- New package: add `<PackageVersion>` to `Directory.Packages.props`, then `<PackageReference Include="..."/>` without a version.
- New service: add the project + `BuildingBlocks` reference, copy a `launchSettings.json`, add a `Services` entry (Development and Production), add a `Gateway:Routes` entry if it is exposed, add it to `.slnLaunch`.

## Commands
```
dotnet build MicroservicesLab.slnx
dotnet run --project src/Services/Employee.Api      # any app; port comes from shared config
```
Verify end to end: start all five, then `GET http://localhost:5000/{employee|payroll|leave}/weatherforecast` → 200, and `http://localhost:5200` lists all three as `200 OK`.
