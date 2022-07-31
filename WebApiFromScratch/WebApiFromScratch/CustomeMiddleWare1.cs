using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApiFromScratch
{
    public class CustomeMiddleWare1 : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("middle custome file ware 1\n");
            await next(context);
        }
    }
}
