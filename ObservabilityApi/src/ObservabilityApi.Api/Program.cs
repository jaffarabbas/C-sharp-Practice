using ObservabilityApi.Api;
using ObservabilityApi.Api.Observability;
using ObservabilityApi.Application;
using ObservabilityApi.Infrastructure;
using Serilog;

LoggingSetup.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var observability = builder.Configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>()
        ?? new ObservabilityOptions();

    builder.AddSerilogLogging(observability);
    builder.AddTelemetry(observability);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.AddWebApi();

    var app = builder.Build();
    app.UseWebApi(observability);

    Log.Information("Starting {Application} {ApplicationVersion} in {Environment}",
        observability.ServiceName, AppInfo.Version, app.Environment.EnvironmentName);

    await app.RunAsync();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException) // HostAbortedException = EF tooling stopping the host.
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    // Flush buffered events (Seq batches) before exit. Bounded: if Seq is unreachable, pending events are dropped.
    await Log.CloseAndFlushAsync();
}

public partial class Program;
