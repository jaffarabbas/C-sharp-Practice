# MicroservicesLab: one place for all settings

Three services, a gateway and a Razor web app that share **one** set of settings. You change a setting, a port, or the database connection string once, and every app picks it up.

```
Employee.Api ─┐
Payroll.Api  ─┼──► ApiGateway (YARP) ◄── WebPortal (Razor)
Leave.Api    ─┘
        all read config/appsettings.Shared*.json
```

## Where everything lives

| What | File | Edit it when |
|---|---|---|
| Settings shared by all apps | `config/appsettings.Shared.json` | Adding or changing any setting |
| Local development values (DB, ports) | `config/appsettings.Shared.Development.json` | Changing your local DB or a port |
| Server values (service addresses) | `config/appsettings.Shared.Production.json` | Before deploying |
| NuGet package versions | `Directory.Packages.props` | Adding or upgrading a package |
| Start all apps in Visual Studio | `MicroservicesLab.slnLaunch` | Adding a new app |
| Launch profile per app | `src/**/Properties/launchSettings.json` | Almost never (all identical) |

No app has its own `appsettings.json`.

---

## 1. Shared appsettings

### Add or change a setting
Put it in `config/appsettings.Shared.json`, then rebuild. Every app gets it.

```json
{
  "Jwt": { "Issuer": "hr-system", "Audience": "hr-clients" },
  "RabbitMq": { "Host": "localhost", "Queue": "hr-events" }
}
```

Read it in code exactly as before, from `IConfiguration` / `IOptions<T>`:

```csharp
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var host = builder.Configuration["RabbitMq:Host"];
```

### Different value per environment
Put the base value in `appsettings.Shared.json` and the override in `appsettings.Shared.Development.json` or `appsettings.Shared.Production.json`. Only the keys that differ go in the environment file.

### One service needs a different value
Create an `appsettings.json` in that one project containing **only that key**. It overrides the shared value for that service only.

### Which value wins
Lowest to highest priority. A later source overrides an earlier one for the same key.

1. `config/appsettings.Shared.json`
2. `config/appsettings.Shared.{Environment}.json`
3. the project's own `appsettings.json` (if it has one)
4. the project's own `appsettings.{Environment}.json`
5. user-secrets (Development only)
6. environment variables
7. command-line arguments

### How it works
`Directory.Build.props` copies the `config/` files into every project's `bin` and publish folder. `builder.AddSharedConfiguration()` (in `src/BuildingBlocks`) loads them at the lowest priority. Every app calls it as the first line after `CreateBuilder`.

> Code that opens `appsettings.json` itself (`new ConfigurationBuilder().AddJsonFile(...)`, often in an EF Core `IDesignTimeDbContextFactory`) does **not** see the shared file. Make it read `IConfiguration` instead, or also load `Path.Combine(AppContext.BaseDirectory, "appsettings.Shared.json")`.

---

## 2. Database connection string

All services use one database, so there is one connection string, `ConnectionStrings:HrDb`.

| Where | How to set it |
|---|---|
| Your machine | `config/appsettings.Shared.Development.json` (local DB, no real passwords) |
| Server (IIS) | Environment variable `ConnectionStrings__HrDb`: system-wide on the server, or in each site's `web.config` under `<environmentVariables>` |
| Under Aspire | Passed to every service automatically from the same shared file |

Read it in code:

```csharp
var cs = builder.Configuration.GetConnectionString("HrDb");
```

**Never commit a real password** in `config/`. Environment variables override the JSON, so the server value always wins.

---

## 3. Launch settings and ports

Visual Studio requires a `launchSettings.json` in every project, so each project keeps one. All of them are **identical** and contain no port:

```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
    }
  }
}
```

Ports live in the `Services` section of `config/appsettings.Shared.Development.json`:

```json
"Services": {
  "ApiGateway":   "http://localhost:5000",
  "Employee.Api": "http://localhost:5101",
  "Payroll.Api":  "http://localhost:5102",
  "Leave.Api":    "http://localhost:5103",
  "WebPortal":    "http://localhost:5200"
}
```

- **Change a port:** edit it here only. The gateway and portal use the same list to find the services, so they follow automatically.
- The key must equal the project's assembly name.
- In Development this port always wins, even over `ASPNETCORE_URLS`. Visual Studio otherwise forces every app onto 5001.
- On servers this code does nothing; IIS decides the binding.

---

## 4. Running everything

| Way | How | Notes |
|---|---|---|
| Visual Studio | Start dropdown → **All services** → F5 | Five apps, five consoles |
| Visual Studio + Aspire | Start dropdown → **Aspire** → F5 | One dashboard with logs, traces, health. Same ports |
| Command line | `dotnet run --project src/Services/Employee.Api` (one terminal per app) | |
| Command line + Aspire | `dotnet run --project src/Aspire/AppHost` | Dashboard link is printed in the console |

Then open http://localhost:5200. The portal shows employee / payroll / leave as `200 OK`.

Aspire is optional. Deleting `src/Aspire` and the `AddServiceDefaults()` / `MapDefaultEndpoints()` lines returns the solution to the plain setup.

---

## 5. Deploying to IIS (manual)

1. `dotnet publish src/Services/Employee.Api -c Release -o <folder>` (repeat per app; **not** the Aspire AppHost).
2. Copy the folder to the IIS site. The shared config files are already inside it.
3. IIS runs as `Production`, so the app loads `appsettings.Shared.json` + `appsettings.Shared.Production.json`.
4. Set `ConnectionStrings__HrDb` on the server.

Before the first deployment:
- [ ] Replace the placeholder addresses in `appsettings.Shared.Production.json` with the real IIS addresses.
- [ ] Remove the placeholder text from `ConnectionStrings:HrDb` in the Production file (leave it empty).

Each IIS site has its **own copy** of the shared files. To change settings on the server in one place, use a server config folder (below).

### One config folder on the server (`SHARED_CONFIG_DIR`)

When the environment variable `SHARED_CONFIG_DIR` points to a folder, every app also loads `appsettings.Shared.json` and `appsettings.Shared.{Environment}.json` from it. That folder overrides the copies inside each app; an app's own `appsettings.json` still wins. If the folder doesn't exist, the app refuses to start with a clear error.

Edit the one file there and recycle the app pools. Set the variable once, either per app pool (what `deploy/create-iis-subapps.ps1` does) or as a system environment variable followed by `iisreset`.

### Office layout: one site with sub-applications

`deploy/create-iis-subapps.ps1` (run as Administrator) builds this from the folders published to `C:\Deploy\MicroserviceTest2`:

| URL | App | App pool |
|---|---|---|
| http://localhost:8900/ | WebPortal (main app) | MicroserviceTest2-WebPortal |
| /gateway | ApiGateway | MicroserviceTest2-ApiGateway |
| /employee-api, /payroll-api, /leave-api | services | one pool each |

`deploy/convert-to-subapps.ps1` (Administrator) removes the five separate sites from the first deployment and builds both `MicroserviceTest` (port 8800) and `MicroserviceTest2` (port 8900) in this layout.

Each .NET Core sub-application needs its **own app pool** (in-process hosting allows one app per pool). Addresses in `SharedConfig\appsettings.Shared.Production.json` are paths on the same host (`http://localhost:8900/gateway`); the template is `deploy/server-config/`.

---

## 6. Adding a new service

1. Create the project and reference `src/BuildingBlocks` and `src/Aspire/ServiceDefaults`.
2. In `Program.cs`: `builder.AddSharedConfiguration();` `builder.AddServiceDefaults();` and, after `Build()`, `app.MapDefaultEndpoints();`
3. Copy any project's `Properties/launchSettings.json` as-is.
4. Add its port to `Services` in the Development file, and its address in the Production file.
5. Exposed through the gateway? Add `"prefix": "ProjectName"` under `Gateway:Routes` in `appsettings.Shared.json`.
6. Add it to `MicroservicesLab.slnLaunch` ("All services"), and an `AddService<...>` line in `src/Aspire/AppHost/AppHost.cs`.

---

## 7. Troubleshooting

| Problem | Fix |
|---|---|
| "Port already in use" | A previous run is still alive. Stop debugging and end leftover `Employee.Api`/… processes in Task Manager. Otherwise pick a free port in the Development file |
| New setting not picked up | Rebuild. The shared files are copied into `bin` at build time |
| Setting works locally, not on the server | Check the Production file, and whether an environment variable on the server overrides it |
| `dotnet ef migrations` can't find the connection string | Your design-time factory reads `appsettings.json` directly (see the note in section 1) |

Background on the design decisions is in `CONTEXT.md`.
