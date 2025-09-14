using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiTemplate.Attributes
{
    public class CustomAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // You can also check route data, model state, etc.
            base.OnActionExecuting(context);
        }
    }
}
