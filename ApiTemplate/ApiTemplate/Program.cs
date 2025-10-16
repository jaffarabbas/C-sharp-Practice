using ApiTemplate.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register all application services (includes Serilog configuration)
builder.AddGeneralDIContainer();

var app = builder.Build();

// Configure middleware pipeline (includes Serilog shutdown handling)
app.UseApplicationPipeline();

Log.Information("Application started successfully");

app.Run();
