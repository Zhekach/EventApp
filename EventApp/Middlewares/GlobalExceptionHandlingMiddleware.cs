using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EventApp.Middlewares;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            if (httpContext.Response.HasStarted)
                throw;
            
            await HandleException(httpContext, exception);
        }
    }

    private async Task HandleException(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception,
            "Unhandled exception. Method={Method}, Path={Path}, RequestId={RequestId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.Request.Headers["x-request-id"]);
        
        var statusCode = StatusCodeMapping(exception);
        
        httpContext.Response.StatusCode = statusCode;

        var error = new ProblemDetails()
        {
            Status = statusCode,
            Detail = statusCode == StatusCodes.Status500InternalServerError 
                ? "Произошла внутренняя ошибка сервера."
                : exception.Message
        };
        
        await httpContext.Response.WriteAsJsonAsync(
            error,
            options: null,
            contentType: "application/problem+json");
    }

    private static int StatusCodeMapping(Exception ex)
        => ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
}