using ApiTemplate.GlobalExceptionHandler.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using KeyNotFoundException = ApiTemplate.GlobalExceptionHandler.Exceptions.KeyNotFoundException;
using NotImplementedException = ApiTemplate.GlobalExceptionHandler.Exceptions.NotImplementedException;
using UnAuthorizedAccessException = ApiTemplate.GlobalExceptionHandler.Exceptions.UnAuthorizedAccessException;

namespace ApiTemplate.GlobalExceptionHandler.Confriguations
{
    public class GlobalExceptionsHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionsHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionsHandlingMiddleware(
            RequestDelegate request,
            ILogger<GlobalExceptionsHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = request;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Custom Error Handler with environment-aware error details
        /// SECURITY: Stack traces are only exposed in Development environment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var status = HttpStatusCode.InternalServerError;
            var message = ex.Message;
            var isDevelopment = _environment.IsDevelopment();

            // Log the full exception details server-side
            _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);

            // Map exception types to HTTP status codes
            switch (ex)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;
                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    break;
                case NotImplementedException:
                    status = HttpStatusCode.NotImplemented;
                    break;
                case UnAuthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    break;
            }

            // Build error response based on environment
            object errorResponse;

            if (isDevelopment)
            {
                // Development: Include detailed error information including stack trace
                errorResponse = new
                {
                    error = message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message,
                    type = ex.GetType().Name,
                    timestamp = DateTime.UtcNow
                };
            }
            else
            {
                // Production: Only include safe, generic error information
                // Never expose stack traces or internal details to clients
                errorResponse = new
                {
                    error = GetSafeErrorMessage(status, message),
                    statusCode = (int)status,
                    timestamp = DateTime.UtcNow,
                    traceId = context.TraceIdentifier // For correlation with server logs
                };
            }

            var exceptionResult = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = isDevelopment
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(exceptionResult);
        }

        /// <summary>
        /// Returns safe, user-friendly error messages for production
        /// Prevents leaking sensitive information about the system
        /// </summary>
        private static string GetSafeErrorMessage(HttpStatusCode status, string originalMessage)
        {
            return status switch
            {
                HttpStatusCode.BadRequest => "The request was invalid. Please check your input and try again.",
                HttpStatusCode.Unauthorized => "Authentication is required to access this resource.",
                HttpStatusCode.Forbidden => "You do not have permission to access this resource.",
                HttpStatusCode.NotFound => "The requested resource was not found.",
                HttpStatusCode.NotImplemented => "This feature is not yet implemented.",
                HttpStatusCode.InternalServerError => "An unexpected error occurred. Please try again later.",
                _ => "An error occurred while processing your request."
            };
        }

    }
}
