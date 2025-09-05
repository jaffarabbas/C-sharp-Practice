using ApiTemplate.Attributes;
using ApiTemplate.BackgroundServices;
using ApiTemplate.Services;
using DBLayer; // added
using FluentValidation;
using FluentValidation.AspNetCore;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database (moved to DBLayer extension)
builder.Services.AddDatabase(builder.Configuration);

// Repositories / UoW
builder.Services.AddRepositoryDI();

// CORS
builder.Services.AddConfiguredCors(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<BGTest>();

builder.Services.AddAuthenticationSchemeService(builder.Configuration);
builder.Services.AddJWTAuthentication(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TblUsersDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplicationPipeline();

app.Run();
