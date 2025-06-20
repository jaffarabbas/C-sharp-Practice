using TestRazor.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddSignalR(); // SignalR support

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7239")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowRazorClient"); // Apply named policy

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ItemNotificationHub>("/itemNotificationHub"); // Map the SignalR hub

app.Run();
