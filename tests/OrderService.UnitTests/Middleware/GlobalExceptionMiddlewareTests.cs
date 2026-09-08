using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OrderService.API.Middleware;

namespace OrderService.UnitTests;

public class GlobalExceptionMiddlewareTests
{
    [Theory]
[InlineData(typeof(UnauthorizedAccessException), 401, "Test exception")]
[InlineData(typeof(KeyNotFoundException), 404, "Test exception")]
[InlineData(typeof(Exception), 500, "An unexpected error occurred.")]
public async Task InvokeAsync_WhenExceptionOccurs_ShouldReturnExpectedResponse(
    Type exceptionType,
    int expectedStatusCode,
    string expectedMessage)
{
        // Arrange
        var context = new DefaultHttpContext();

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var exception = (Exception)Activator.CreateInstance(
            exceptionType,
            "Test exception")!;

        RequestDelegate next = _ =>
            throw exception;

        var middleware = new GlobalExceptionMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            context.Response.ContentType);

        responseBody.Position = 0;

        using var document = await JsonDocument.ParseAsync(responseBody);

        var root = document.RootElement;

        Assert.Equal(
            expectedStatusCode,
            root.GetProperty("statusCode").GetInt32());

        Assert.Equal(
            expectedMessage,
            root.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenNoExceptionOccurs_ShouldCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }
}