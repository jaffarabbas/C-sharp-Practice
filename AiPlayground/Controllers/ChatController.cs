using Microsoft.AspNetCore.Mvc;
using AiPlayground.Repository;

namespace AiPlayground.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IChatClient _chatClient;

    public ChatController(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    [HttpPost(Name = "ChatWithModelBody")]
    public async Task<ActionResult<ChatResponse>> Post(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message is required.");
        }

        var response = await _chatClient.GetResponseAsync(request.Message, cancellationToken);
        return Ok(response);
    }

    [HttpGet(Name = "ChatWithModel")]
    public async Task<IActionResult> Get(
        [FromQuery] string message,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("Message is required.");
        }

        var response = await _chatClient.GetResponseAsync(message, cancellationToken);
        return Ok(response);
    }
}
