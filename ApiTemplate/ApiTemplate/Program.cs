using ApiTemplate.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGeneralDIContainer(builder.Configuration);

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();
