using ApiTemplate.Services;
using Shared.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early in the pipeline
builder.ConfigureSerilog();

builder.Services.AddGeneralDIContainer(builder.Configuration);

var app = builder.Build();

// Ensure Serilog is properly closed on shutdown
app.EnsureSerilogClosed();

app.UseApplicationPipeline();

Log.Information("Application started successfully");

app.Run();
