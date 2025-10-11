using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Services;

namespace Repositories.Attributes
{
    /// <summary>
    /// Authorization attribute that checks if the user has the required permission to access the resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        public string ResourceName { get; set; }
        public string ActionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the RequirePermissionAttribute.
        /// </summary>
        /// <param name="resourceName">The name of the resource (e.g., "Employee", "Invoice")</param>
        /// <param name="actionType">The action type (e.g., "view", "add", "edit", "delete")</param>
        public RequirePermissionAttribute(string resourceName, string actionType)
        {
            ResourceName = resourceName;
            ActionType = actionType;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the authorization service from DI
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

            if (authorizationService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Get the user ID from claims
            var userIdClaim = context.HttpContext.User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "User not authenticated",
                    Timestamp = DateTime.UtcNow
                });
                return;
            }

            // Check if user has permission
            var hasPermission = await authorizationService.HasPermissionAsync(userId, ResourceName, ActionType);

            if (!hasPermission)
            {
                context.Result = new ObjectResult(new
                {
                    Message = $"Access denied. You do not have permission to {ActionType} {ResourceName}.",
                    Resource = ResourceName,
                    Action = ActionType,
                    Timestamp = DateTime.UtcNow
                })
                {
                    StatusCode = 403 // Forbidden
                };
                return;
            }

            // User has permission, continue with the action
            await next();
        }
    }
}
