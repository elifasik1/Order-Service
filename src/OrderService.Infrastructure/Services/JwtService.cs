using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Settings;
using System.Security.Cryptography;


namespace OrderService.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public Task<string> GenerateAccessTokenAsync(User user)
    {
    byte[] keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
    var key = new SymmetricSecurityKey(keyBytes);
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.UserRole.ToString())
    };
    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        signingCredentials: credentials
    );  
    var tokenHandler = new JwtSecurityTokenHandler();
    return Task.FromResult(tokenHandler.WriteToken(token));

    
}

public string GenerateRefreshToken()
{
    var randomBytes = new byte[32];

    RandomNumberGenerator.Fill(randomBytes);

    return Convert.ToBase64String(randomBytes);
}
}