using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApiFromScratch
{
    public class CustomeMiddleWare1 : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new System.NotImplementedException();
        }
    }
}
