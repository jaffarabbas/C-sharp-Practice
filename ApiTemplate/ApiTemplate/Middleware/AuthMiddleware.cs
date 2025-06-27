namespace TestApi.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            //var token = context.Request.Headers["Authorization"].ToString();
            //if (string.IsNullOrEmpty(token))
            //{
            //    context.Response.StatusCode = 401;
            //    await context.Response.WriteAsync("Unauthorized");
            //    return;
            //}
            // Validate the token here
            // If valid, call the next middleware
            await _next(context);
        }
    }
}
