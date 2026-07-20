using System.Diagnostics;
using System.Net;
using System.Text.Json;
using ProductApp.Api.Exceptions;
using ProductApp.Shared.Contracts;

namespace ProductApp.Api.Middleware;

/// <summary>
/// It will catch unhandled exception 
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, code, message) = exception switch
        {
            ProductNotFoundException notFound => (HttpStatusCode.NotFound, "PRODUCT_NOT_FOUND", notFound.Message),
            ValidationException validation => (HttpStatusCode.BadRequest, "VALIDATION_ERROR", validation.Message),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred while processing the request.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, traceId);
        }
        else
        {
            _logger.LogWarning("Request {Method} {Path} failed with {StatusCode}: {Message}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, (int)statusCode, message, traceId);
        }

        var response = new ApiErrorResponse
        {
            Code = code,
            Message = message,
            StatusCode = (int)statusCode,
            TraceId = traceId
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
