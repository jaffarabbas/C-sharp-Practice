using BuildingBlocks;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedConfiguration();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("gateway", client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:ApiGateway"]!.TrimEnd('/') + "/"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
