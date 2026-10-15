using System.ClientModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
var aiSettings = builder.Configuration.GetSection("AISettings").Get<AISettings>() ?? new AISettings();

builder.Services.AddOpenApi();
builder.Services.AddChatClient(
    new OpenAIClient(
        new ApiKeyCredential(aiSettings?.Platform ?? string.Empty),
        new OpenAIClientOptions
        {
            Endpoint = new Uri(aiSettings?.OpenAIEndpoint ?? string.Empty)
        })
    .GetChatClient(aiSettings?.OpenAIModel)
    .AsIChatClient());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}

app.MapGet("/chat", async ([FromQuery] string message, IChatClient chatClient, CancellationToken cancellationToken) =>
{
    var response = await chatClient.GetResponseAsync(
        message,
        cancellationToken: cancellationToken);

    return Results.Text(response.Text);
}).WithName("ChatWithModel");

app.Run();

