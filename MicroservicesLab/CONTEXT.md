# Context: centralizing microservice configuration

## The problem
The user's office project is an ASP.NET Core microservices solution. Every service carries its own `appsettings.json`, `appsettings.{Env}.json` and `Properties/launchSettings.json`. Changing a setting means editing it in every service.

Facts about that project (as stated by the user):
- **One database shared by all services.** Same connection string everywhere.
- **All settings are the same in every service.** The per-service files are copies of each other.
- It has several services, a gateway, and a web front end (similar shape to this lab).

Unknown until the office code is inspected: .NET version, whether `Program.cs` uses `WebApplication.CreateBuilder` (minimal hosting, .NET 6+) or `Host.CreateDefaultBuilder` + `Startup`, which gateway (YARP / Ocelot / custom), how it is deployed (IIS, Docker, Kubernetes, App Service), and where secrets live today.

## Goal
Edit a setting once and have every service get it. No settings in `launchSettings.json` beyond the environment name.

## Decisions and why

| Decision | Why |
|---|---|
| One set of files in `config/` (`appsettings.Shared.json`, `.Development.json`, `.Production.json`) | Settings are identical across services, so per-service files are pure duplication. |
| `Directory.Build.props` links those files into every project and copies them to `bin` | Zero per-project csproj edits; publish output contains them too. |
| Load them from `AppContext.BaseDirectory`, not content root | During `dotnet run` / F5 the content root is the project folder, where the linked files do not physically exist. `bin` always has them. |
| Insert the shared sources at index 0 and 1 of `Configuration.Sources` | Lowest priority, so a service's own `appsettings.json` (if ever added), user-secrets, environment variables and command-line args still override. Appending would make the shared file override env vars, which breaks server deployments. |
| Ports in `Services:<AssemblyName>`; app calls `UseUrls` with its own entry | `launchSettings.json` cannot be shared (VS and `dotnet run` read it only from each project's `Properties/` folder). Moving the port out leaves those files identical and never edited. The same map is how the gateway and web app find services. |
| `UseUrls` only in Development, and only if `urls`/`http_ports` are not already set | In Production `Services:*` holds addresses others call (`http://employee-api:8080`), not bind addresses. Docker sets `ASPNETCORE_HTTP_PORTS`; IIS controls binding itself. |
| Gateway routes built in code from `Gateway:Routes` + `Services:*` | YARP's JSON config needs literal addresses, which would duplicate the service URLs. |
| `Directory.Packages.props` (central package management) with transitive pinning | One place for versions. Also used to pin `Microsoft.OpenApi` to 2.13.0: the .NET 10 webapi template pulls 2.0.0, which has advisory GHSA-v5pm-xwqc-g5wc (patched in 2.7.5). Stay on 2.x; `Microsoft.AspNetCore.OpenApi` 10 targets 2.x. |
| `.slnLaunch` for starting everything | Replaces per-developer "multiple startup projects" settings with a committed file. Needs VS 2022 17.10+. |
| .NET Aspire AppHost considered, not chosen | Would also centralize ports and startup, but adds a new project type and workload. The simple approach was enough once settings turned out to be identical. Revisit if the office team wants dashboards/service discovery. |

## Verified in the lab (2026-10-21)
- Build: 0 warnings, 0 errors.
- All five apps start on the ports from the shared Development file (5101/5102/5103/5000/5200).
- Gateway → each service returns 200; Razor portal → gateway → services shows 200 for all three.

Not verified: the Production file, Docker, IIS, and the `IHostBuilder` variant in the skill.

## Open items for the office project
- Confirm the "all settings identical" claim by diffing the actual files before deleting anything.
- Real secrets must not go into the shared JSON: user-secrets locally, environment variables / Key Vault on servers.
- A master `launchSettings.json` copied into each project by a `Directory.Build.targets` step was offered but not built.
