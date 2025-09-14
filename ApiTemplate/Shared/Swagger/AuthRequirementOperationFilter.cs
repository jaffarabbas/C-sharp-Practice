using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiTemplate.Swagger
{
    public class AuthRequirementOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if the action or controller has SkipJwtValidation attribute
            var hasSkipJwtValidation = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<SkipJwtValidationAttribute>().Any() == true ||
                context.MethodInfo.GetCustomAttributes(true)
                .OfType<SkipJwtValidationAttribute>().Any() == true;

            // Check if the action or controller has AllowAnonymous attribute
            var hasAllowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any() == true ||
                context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any() == true;

            // If it has SkipJwtValidation or AllowAnonymous, don't require authentication
            if (hasSkipJwtValidation || hasAllowAnonymous)
            {
                return;
            }

            // Check if the action or controller has Authorize attribute
            var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>().Any() == true ||
                context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>().Any() == true;

            if (hasAuthorize)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [bearerScheme] = new List<string>()
                    }
                };
            }
        }
    }
}
