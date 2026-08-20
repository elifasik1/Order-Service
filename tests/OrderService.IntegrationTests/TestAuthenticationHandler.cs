using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderService.IntegrationTests;

public class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "11111111-1111-1111-1111-111111111111"),

            new Claim(
                ClaimTypes.Email,
                "admin@orderservice.com"),

            new Claim(
                ClaimTypes.Role,
                "Admin"),

            new Claim(
                ClaimTypes.Name,
                "System Admin")
        };

        var identity = new ClaimsIdentity(
            claims,
            "Test");

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            "Test");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}