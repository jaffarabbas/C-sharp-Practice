using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ObservabilityApi.Application.Common;
using ObservabilityApi.Domain;

namespace ObservabilityApi.Api.ErrorHandling;

/// <summary>
/// The single place where exceptions become HTTP responses and where unexpected exceptions are logged.
/// Nothing below this boundary logs-and-rethrows, so each failure is logged exactly once.
/// Expected rejections (validation, conflicts) are not errors: they are logged at Information by the
/// application layer and visible in the request completion event, without stack traces.
/// </summary>
internal sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // Client closed the connection; nginx's de-facto code. Nothing is written to the (gone) client.
    private const int ClientClosedRequest = 499;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            LogRequestAborted(logger);
            httpContext.Response.StatusCode = ClientClosedRequest;
            return true;
        }

        ProblemDetails problem = exception switch
        {
            ValidationException validation => new HttpValidationProblemDetails(ToErrorDictionary(validation))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
            },
            DomainException domain => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The request violates a business rule.",
                Detail = domain.Message,
            },
            ConflictException conflict => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "The request conflicts with the current state of the resource.",
                Detail = conflict.Message,
            },
            BadHttpRequestException badRequest => new ProblemDetails
            {
                Status = badRequest.StatusCode,
                Title = "The request is malformed.",
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                // No exception message or stack trace in the response; use traceId to find it in Seq/Jaeger.
                Detail = "The error has been logged. Quote the traceId when reporting this problem.",
            },
        };

        if (problem.Status >= StatusCodes.Status500InternalServerError)
        {
            LogUnhandledException(logger, exception, httpContext.Request.Method, httpContext.Request.Path);
            // ASP.NET Core 10 does not emit exception diagnostics for handled exceptions, so record it on the span.
            Activity.Current?.AddException(exception);
        }

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
        });
    }

    private static Dictionary<string, string[]> ToErrorDictionary(ValidationException exception) =>
        exception.Errors
            .GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

    [LoggerMessage(EventId = 5000, EventName = "UnhandledException", Level = LogLevel.Error,
        Message = "Unhandled exception while processing {RequestMethod} {RequestPath}")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception, string requestMethod, string requestPath);

    [LoggerMessage(EventId = 5001, EventName = "RequestAborted", Level = LogLevel.Debug,
        Message = "Request aborted by the client")]
    private static partial void LogRequestAborted(ILogger logger);
}
