using ApiTemplate.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;
using ApiTemplate.Attributes;
using ApiTemplate.BackgroundServices;
using ApiTemplate.Hubs;
using ApiTemplate.Middleware;
using ApiTemplate.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TestContext>(options =>
{

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IITemRepository, ItemRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


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

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

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

app.MapHub<ItemNotificationHub>("/itemNotificationHub");

app.Run();
