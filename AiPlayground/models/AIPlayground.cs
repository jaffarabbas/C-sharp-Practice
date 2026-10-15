using System.ComponentModel.DataAnnotations;

public record AISettings
{
    public string? OpenAIKey { get; set; } = "";
    public string? Platform { get; set; } = "";
    public string? OpenAIEndpoint { get; set; } = "";
    public string? OpenAIModel { get; set; } = "";
}

public class ChatResponse
{
    public string Text { get; set; } = string.Empty;
}

public class ChatRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}
