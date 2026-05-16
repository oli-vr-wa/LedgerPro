
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LedgerPro.Core.Exceptions;

namespace LedgerPro.Api.Middleware;

/// <summary>
/// A global exception handler that implements the IExceptionHandler interface to handle exceptions across the entire application.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles exceptions globally and returns appropriate HTTP responses based on the type of exception.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A value indicating whether the exception was handled.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "LedgerPro handled an error: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
        };

        // Map known exceptions to appropriate status codes and messages
        switch (exception)
        {
            case ArgumentException or InvalidOperationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Something went wrong - Bad Request";
                problemDetails.Detail = exception.Message;
                break;

            case BusinessException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Business Rules Violation - Bad Request";
                problemDetails.Detail = exception.Message;
                break;
                
            // Add more specific exception types and mappings as needed

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Unexpected Error - Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indicate that the exception has been handled
    }
}
