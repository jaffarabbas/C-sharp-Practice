using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Service;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly ILeaveService _leaveService;

    public ChatController(Kernel kernel, ILeaveService leaveService)
    {
        _kernel = kernel;
        _leaveService = leaveService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var result = await _kernel.InvokePromptAsync(request.Message);
        return Ok(new { response = result.ToString() });
    }

    [HttpPost("with-leave")]
    public async Task<IActionResult> ChatWithLeave([FromBody] ChatRequest request)
    {
        // Import the plugin only if it doesn't exist
        if (!_kernel.Plugins.Contains("HrmsPlugin"))
        {
            _kernel.ImportPluginFromObject(new HrmsPlugin(_leaveService), "HrmsPlugin");
        }

        // Enable automatic function calling
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var result = await _kernel.InvokePromptAsync(
            request.Message,
            new KernelArguments(executionSettings));
            
        return Ok(new { response = result.ToString() });
    }
}

public class ChatRequest
{
    public required string Message { get; set; }
}
