using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiTemplate.Dtos;
using Microsoft.AspNetCore.Http;

public class ApiHitResponse : IActionResult
{
    private readonly Object _value;
    private string _message;
    private int _statusCode;

    public ApiHitResponse(Object value, string message,int statusCode)
    {
        _value = value;
        this._message = message;
        this._statusCode = statusCode;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = "application/json";
        var responseData = new ApiResponse<Object>
        {
            Message = this._message,
            StatusCode = _statusCode.ToString(),
            Data = this._value
        };
        await response.WriteAsync(JsonConvert.SerializeObject(responseData));
    }
}