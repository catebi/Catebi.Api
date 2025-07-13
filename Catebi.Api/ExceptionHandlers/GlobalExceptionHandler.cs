using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "⚠️ Unhandled exception occurred");

        httpContext.Response.StatusCode = exception switch
        {
            ArgumentNullException       => StatusCodes.Status400BadRequest,
            ArgumentException           => StatusCodes.Status400BadRequest,
            InvalidOperationException   => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            NotSupportedException       => StatusCodes.Status501NotImplemented,
            TimeoutException            => StatusCodes.Status408RequestTimeout,
            FileNotFoundException       => StatusCodes.Status404NotFound,
            DirectoryNotFoundException  => StatusCodes.Status404NotFound,
            _                           => StatusCodes.Status500InternalServerError
        };

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = GetProblemType(exception),
                Title = GetProblemTitle(exception),
                Detail = GetProblemDetail(exception, httpContext),
                Status = httpContext.Response.StatusCode,
                Instance = httpContext.Request.Path
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }

    private static string GetProblemType(Exception exception) => exception switch
    {
        ArgumentNullException       => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ArgumentException           => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        InvalidOperationException   => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        UnauthorizedAccessException => "https://tools.ietf.org/html/rfc7235#section-3.1",
        NotSupportedException       => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
        TimeoutException            => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
        FileNotFoundException       => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        DirectoryNotFoundException  => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        _                           => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };

    private static string GetProblemTitle(Exception exception) => exception switch
    {
        ArgumentNullException       => "Bad Request",
        ArgumentException           => "Bad Request",
        InvalidOperationException   => "Bad Request",
        UnauthorizedAccessException => "Unauthorized",
        NotSupportedException       => "Not Implemented",
        TimeoutException            => "Request Timeout",
        FileNotFoundException       => "Not Found",
        DirectoryNotFoundException  => "Not Found",
        _                           => "Internal Server Error"
    };

    private static string GetProblemDetail(Exception exception, HttpContext httpContext)
    {
        var environment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        
        if (environment.IsDevelopment())
        {
            return exception.Message;
        }

        return exception switch
        {
            ArgumentNullException       => "Required parameter is missing.",
            ArgumentException           => "The request contains invalid arguments.",
            InvalidOperationException   => "The operation is not valid in the current state.",
            UnauthorizedAccessException => "Access is denied.",
            NotSupportedException       => "The operation is not supported.",
            TimeoutException            => "The request timed out.",
            FileNotFoundException       => "The file was not found.",
            DirectoryNotFoundException  => "The directory was not found.",
            _                           => "An error occurred while processing your request."
        };
    }
} 