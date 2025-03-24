using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAppUtils.ExceptionHandlers;

public class OperationCanceledExceptionHandler : IExceptionHandler
{
    private readonly ILogger<OperationCanceledExceptionHandler> _logger;

    public OperationCanceledExceptionHandler(ILogger<OperationCanceledExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not OperationCanceledException e)
        {
            return false;
        }

        _logger.LogInformation("Operation was canceled: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);
        
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status200OK,
            Detail = $"Operation was canceled: {exception.Message}"
        };

        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}