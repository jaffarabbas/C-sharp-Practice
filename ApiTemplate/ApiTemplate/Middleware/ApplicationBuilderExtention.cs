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
        public static IApplicationBuilder UseApiResponse(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<ApiResponseMiddleware>();
    }
}
