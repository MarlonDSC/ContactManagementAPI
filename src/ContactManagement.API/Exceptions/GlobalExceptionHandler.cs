using System.Diagnostics;
using ContactManagement.Shared.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            ResultException resultException => (int)resultException.StatusCode,
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        string? detail = exception switch
        {
            ResultException resultException => resultException.Error.Message,
            _ => exception.Message
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Status = httpContext.Response.StatusCode,
                Detail = detail,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = new Dictionary<string, object?>
                {
                    { "requestId", httpContext.TraceIdentifier },
                    { "traceId", activity?.Id }
                }
            }
        });
    }
}