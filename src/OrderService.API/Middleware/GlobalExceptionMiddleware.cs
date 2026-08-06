
using System.Text.Json;
using OrderService.API.DTOs;
namespace OrderService.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
private static async Task HandleExceptionAsync(
    HttpContext context,
    Exception ex)
{
    context.Response.ContentType = "application/problem+json";

    var statusCode = ex switch
    {
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };

    context.Response.StatusCode = statusCode;

    var response = new ErrorResponse
    {
        StatusCode = statusCode,
        Message = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : ex.Message,
        Timestamp = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(
    response,
    new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    await context.Response.WriteAsync(json);
}

    
}