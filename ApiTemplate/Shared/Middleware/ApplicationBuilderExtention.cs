using ApiTemplate.GlobalExceptionHandler.Confriguations;
using CustomMiddlewareCollection.ValidateTokenMiddleware.Confriguations;
using Microsoft.AspNetCore.Builder;

namespace ApiTemplate.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Custom Auth Middleware
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<AuthMiddleware>();
        /// <summary>
        /// Use Api Generic Response
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiResponse(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<ApiResponseMiddleware>();
        /// <summary>
        /// Custom middleware for exception handling
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<GlobalExceptionsHandlingMiddleware>();

        /// <summary>
        /// Global permission checking middleware based on resource ID from headers
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<PermissionMiddleware>();

        /// <summary>
        /// SECURITY: Adds comprehensive security headers to all responses
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<SecurityHeadersMiddleware>();

        /// <summary>
        /// Performance monitoring middleware to track request processing times
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePerformanceMonitoringMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<PerformanceMonitoringMiddleware>();

        /// <summary>
        /// Request and Response logging middleware
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLoggingMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
