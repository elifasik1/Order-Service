using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OrderService.IntegrationTests;

public class OrdersEndpointTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

[Fact]
public async Task GetOrders_AuthenticatedUser_ShouldReturnSuccess()
{
    // Act
    var response = await _client.GetAsync("/orders?page=1&pageSize=10");

    var content = await response.Content.ReadAsStringAsync();

    // Assert
    Assert.True(
        response.IsSuccessStatusCode,
        $"Status: {(int)response.StatusCode} {response.StatusCode}\nResponse: {content}");
}
}