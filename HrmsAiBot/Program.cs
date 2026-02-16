using Microsoft.SemanticKernel;
using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILeaveService, LeaveService>();

builder.Services.AddSingleton<Kernel>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var kernelBuilder = Kernel.CreateBuilder();

    var openAiModelId = config["OpenAI:ModelId"] ?? "gpt-4o-mini";
    var openAiApiKey = config["OpenAI:ApiKey"];

    if (string.IsNullOrEmpty(openAiApiKey))
    {
        throw new InvalidOperationException("OpenAI API key is not configured. Set it using User Secrets (dev) or environment variables (production).");
    }

    kernelBuilder.AddOpenAIChatCompletion(
        modelId: openAiModelId,
        apiKey: openAiApiKey);

    var kernel = kernelBuilder.Build();

    return kernel;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
