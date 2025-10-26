using ApiTemplate.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register all application services (includes Serilog configuration)
builder.AddGeneralDIContainer();

var app = builder.Build();

// Apply database migrations before configuring the pipeline
app.ApplyDatabaseMigrations();

// Configure middleware pipeline (includes Serilog shutdown handling and permission checking)
app.UseApplicationPipeline();

Log.Information("Application started successfully");

app.Run();
