using ApiTemplate.Attributes;
using ApiTemplate.BackgroundServices;
using ApiTemplate.Hubs;
using ApiTemplate.Middleware;
using ApiTemplate.Repository;
using ApiTemplate.Services;
using AutoMapper;
using DBLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TestContext>(options =>
{

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDIContainer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<BGTest>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7239",       // Razor Pages (if still needed)
            "http://127.0.0.1:5501",        // Your HTML/Tabulator app
            "http://localhost:5501")        // (optional) for localhost fallback) // your Razor page's port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // REQUIRED for SignalR with cross-origin
    });
});

builder.Services.AddSignalR();

builder.Services.AddMemoryCache();
    
builder.Services.AddJWTAuthentication(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TblUsersDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowRazorClient");

app.UseHttpsRedirection();

app.UseApiResponse();

app.UseAuthorization();

app.MapControllers();

app.UseAuthMiddleware();

app.UseExceptionMiddleware();

app.UseValidateTokenHandler();

app.MapHub<ItemNotificationHub>("/itemNotificationHub");

app.Run();
