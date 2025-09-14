using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class SkipJwtValidationAttribute : Attribute
{
    /* no code needed – it’s just a marker */
}
