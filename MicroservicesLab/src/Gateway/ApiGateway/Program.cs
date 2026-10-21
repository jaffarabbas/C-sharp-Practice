using BuildingBlocks;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedConfiguration();
builder.AddServiceDefaults();

// Routes come from Gateway:Routes (path prefix -> service name); addresses come from Services:<name>,
// so a service's URL is defined once in the shared config and nowhere else.
var routeMap = builder.Configuration.GetSection("Gateway:Routes").GetChildren()
    .ToDictionary(r => r.Key, r => r.Value!);

var routes = routeMap.Select(r => new RouteConfig
{
    RouteId = r.Key,
    ClusterId = r.Value,
    Match = new RouteMatch { Path = $"/{r.Key}/{{**rest}}" }
}.WithTransformPathRemovePrefix($"/{r.Key}")).ToList();

var clusters = routeMap.Values.Distinct().Select(service => new ClusterConfig
{
    ClusterId = service,
    Destinations = new Dictionary<string, DestinationConfig>
    {
        ["default"] = new() { Address = builder.Configuration[$"Services:{service}"]! }
    }
}).ToList();

builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "ApiGateway");
app.MapReverseProxy();

app.Run();
