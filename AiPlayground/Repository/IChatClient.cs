using Microsoft.Extensions.AI;
using AiPlayground.Tools;

namespace AiPlayground.Repository;

public interface IChatClient
{
    Task<ChatResponse> GetResponseAsync(string message, CancellationToken cancellationToken = default);
}

public class ChatClient(Microsoft.Extensions.AI.IChatClient chatClient) : IChatClient
{
    private readonly Microsoft.Extensions.AI.IChatClient _chatClient = chatClient;

    public async Task<ChatResponse> GetResponseAsync(string message, CancellationToken cancellationToken = default)
    {
        var response = await _chatClient.GetResponseAsync(
            message,
            options: new ChatOptions
            {
                Tools = [AIFunctionFactory.Create(CalculatorTools.Add)]
            },
            cancellationToken: cancellationToken);

        return new ChatResponse
        {
            Text = response.Text
        };
    }
}
