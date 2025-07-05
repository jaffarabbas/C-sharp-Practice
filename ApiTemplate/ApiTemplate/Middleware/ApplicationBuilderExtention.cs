using ApiTemplate.GlobalExceptionHandler.Confriguations;

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
    }
}
