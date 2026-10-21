using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

var builder = DistributedApplication.CreateBuilder(args);

// Same config/appsettings.Shared*.json the services use (copied into bin by Directory.Build.props),
// so ports and the connection string are identical whether you start via Aspire or without it.
var files = new PhysicalFileProvider(AppContext.BaseDirectory);
builder.Configuration.Sources.Insert(0, new JsonConfigurationSource { FileProvider = files, Path = "appsettings.Shared.json" });
builder.Configuration.Sources.Insert(1, new JsonConfigurationSource
{
    FileProvider = files, Path = $"appsettings.Shared.{builder.Environment.EnvironmentName}.json", Optional = true
});

var hrDb = builder.AddConnectionString("HrDb");

// Aspire's proxy listens on the port from Services:<ProjectName>; the app itself gets a port from Aspire.
// Hosting__Orchestrated tells AddSharedConfiguration() not to bind that port itself.
IResourceBuilder<ProjectResource> AddService<TProject>(string name, string projectName)
    where TProject : IProjectMetadata, new() =>
    builder.AddProject<TProject>(name)
        .WithHttpEndpoint(port: new Uri(builder.Configuration[$"Services:{projectName}"]!).Port)
        .WithEnvironment("Hosting__Orchestrated", "true")
        .WithReference(hrDb);

var employee = AddService<Projects.Employee_Api>("employee-api", "Employee.Api");
var payroll = AddService<Projects.Payroll_Api>("payroll-api", "Payroll.Api");
var leave = AddService<Projects.Leave_Api>("leave-api", "Leave.Api");

var gateway = AddService<Projects.ApiGateway>("api-gateway", "ApiGateway")
    .WithReference(employee).WaitFor(employee)
    .WithReference(payroll).WaitFor(payroll)
    .WithReference(leave).WaitFor(leave);

AddService<Projects.WebPortal>("web-portal", "WebPortal")
    .WithReference(gateway).WaitFor(gateway);

builder.Build().Run();
