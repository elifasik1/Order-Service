using Microsoft.AspNetCore.Mvc.Testing;
using OrderService.API;

namespace OrderService.IntegrationTests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
}