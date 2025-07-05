using ApiTemplate.Dtos;
using Newtonsoft.Json;
using Shared.Helper;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace ApiTemplate.Middleware
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Save the original response body stream
            var originalBodyStream = context.Response.Body;

            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                // Continue down the pipeline
                await _next(context);

                // Reset and optionally decompress the stream
                memStream.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(memStream).ReadToEndAsync();

                object data;
                try
                {
                    data = JsonConvert.DeserializeObject<object>(bodyText);
                }
                catch
                {
                    data = bodyText; // fallback to plain string
                }
                // Rebuild standardized response
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = context.Response.StatusCode.ToString(),
                    Message = ApiResponseMessage.GetMessageFromStatus(context.Response.StatusCode),
                    Data = data!
                };

                var json = JsonConvert.SerializeObject(apiResponse);

                // Write back to original stream
                context.Response.ContentType = "application/json";
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(json);
                context.Response.Body = originalBodyStream;

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBodyStream;
                context.Response.StatusCode = 500;
                var errorResponse = new ApiResponse<string>
                {
                    StatusCode = "500",
                    Message = "Server Error",
                    Data = ex.Message
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
        }
    }
}
