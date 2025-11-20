using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Exceptions;
using System.Diagnostics;
using System.Net;

namespace Remp.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = title,
            Status = (int)statusCode,
            Detail = httpContext.RequestServices
                                .GetRequiredService<IHostEnvironment>()
                                .IsDevelopment()
                                ? exception.Message : null,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
        problemDetails.Extensions.Add("timestamp", DateTime.UtcNow);

        // Logging
        logger.LogError(
            exception,
            $"exception occurred. Title: {problemDetails.Title}, Detail: {problemDetails.Detail}, Status: {problemDetails.Status}, Path: {problemDetails.Instance}, TraceId: {httpContext.TraceIdentifier}");

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }

    // Specific Exception Mappings
    private (HttpStatusCode StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException ne => (ne.StatusCode, ne.Title),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };
    }
}
