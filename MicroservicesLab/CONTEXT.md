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
| `UseUrls` only in Development, and skipped when `ASPNETCORE_HTTP_PORTS` is set | In Production `Services:*` holds addresses others call (`http://employee-api:8080`), not bind addresses. Docker sets `ASPNETCORE_HTTP_PORTS`; IIS controls binding itself. |
| In Development the shared port **overrides** `ASPNETCORE_URLS` | Visual Studio sets `ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000` when a launch profile has no `applicationUrl`. The first version deferred to it, so in VS every service tried 5001 ("address already in use"); `dotnet run` was unaffected, which hid the bug. Reproduced and fixed 2026-10-22. Side effect: `--urls` on the command line is ignored in Development; change the port in the shared file instead. |
| Gateway routes built in code from `Gateway:Routes` + `Services:*` | YARP's JSON config needs literal addresses, which would duplicate the service URLs. |
| `Directory.Packages.props` (central package management) with transitive pinning | One place for versions. Also used to pin `Microsoft.OpenApi` to 2.13.0: the .NET 10 webapi template pulls 2.0.0, which has advisory GHSA-v5pm-xwqc-g5wc (patched in 2.7.5). Stay on 2.x; `Microsoft.AspNetCore.OpenApi` 10 targets 2.x. |
| `.slnLaunch` for starting everything | Replaces per-developer "multiple startup projects" settings with a committed file. Needs VS 2022 17.10+. |
| .NET Aspire added as an **optional** second way to run (2026-10-22), not a replacement | The user asked that it not affect the existing setup. AppHost reads the same shared config, so ports and connection string have one source. Its proxy listens on the shared-config ports, so the gateway/portal `Services:` URLs work unchanged in both modes. AppHost sets `Hosting__Orchestrated=true`; `AddSharedConfiguration()` then skips `UseUrls` so it does not fight Aspire's port assignment. |
| `AddServiceDefaults()` in every app | Gives the Aspire dashboard logs/traces/metrics. Safe without Aspire: the OTLP exporter only turns on when `OTEL_EXPORTER_OTLP_ENDPOINT` is set. Side effects in both modes: `/health` and `/alive` endpoints in Development, and retries/timeouts (standard resilience handler) on every `HttpClient`. |

## Verified in the lab (2026-10-21)
- Build: 0 warnings, 0 errors.
- All five apps start on the ports from the shared Development file (5101/5102/5103/5000/5200).
- Gateway → each service returns 200; Razor portal → gateway → services shows 200 for all three.

- 2026-10-22, after adding Aspire: the five-app startup (with VS-style `ASPNETCORE_URLS` set) still binds 5101/5102/5103/5000/5200 and the portal shows 200 for all; `/health` returns Healthy. Starting only the AppHost: the portal on 5200 and the gateway on 5000 return 200 for all three services; the apps themselves run on Aspire-assigned ports (2048x).

Not verified: the Production file, Docker, IIS, the `IHostBuilder` variant in the skill, and Aspire started from Visual Studio (only tested with `dotnet run`).

## Open items for the office project
- Confirm the "all settings identical" claim by diffing the actual files before deleting anything.
- Real secrets must not go into the shared JSON: user-secrets locally, environment variables / Key Vault on servers.
- A master `launchSettings.json` copied into each project by a `Directory.Build.targets` step was offered but not built.
