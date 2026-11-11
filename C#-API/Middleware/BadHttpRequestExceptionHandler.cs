using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer_API.Middleware;

public sealed class BadHttpRequestExceptionHandler(
    ILogger<BadHttpRequestExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequestException)
        {
            return false;
        }

        logger.LogWarning(badRequestException, "Rejected malformed HTTP request.");

        var statusCode = badRequestException.StatusCode;
        var title = statusCode switch
        {
            StatusCodes.Status413PayloadTooLarge => "Payload Too Large",
            StatusCodes.Status415UnsupportedMediaType => "Unsupported Media Type",
            _ => "Bad Request"
        };

        httpContext.Response.StatusCode = statusCode;
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = "The request could not be processed.",
                Instance = httpContext.Request.Path
            }
        });

        return true;
    }
}
