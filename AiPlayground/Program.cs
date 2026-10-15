using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
var aiSettings = builder.Configuration.GetSection("AISettings").Get<AISettings>() ?? new AISettings();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddChatClient(
    new OpenAIClient(
        new ApiKeyCredential(string.IsNullOrWhiteSpace(aiSettings.OpenAIKey) ? "lm-studio" : aiSettings.OpenAIKey),
        new OpenAIClientOptions
        {
            Endpoint = new Uri(aiSettings.OpenAIEndpoint ?? string.Empty)
        })
    .GetChatClient(aiSettings.OpenAIModel)
    .AsIChatClient()).UseFunctionInvocation();

builder.Services.AddScoped<AiPlayground.Repository.IChatClient, AiPlayground.Repository.ChatClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

