using OrderService.Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace OrderService.Application.Features.Auth.Login;
public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
    }

   public async Task<LoginResponse> Handle(LoginRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = await _jwtService.GenerateAccessTokenAsync(user);
       var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
{
    Id = Guid.NewGuid(),
    Token = refreshTokenValue,
    UserId = user.Id,
    CreatedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddDays(7),
    RevokedAt = null
};
await _refreshTokenRepository.AddAsync(refreshToken);
await _refreshTokenRepository.SaveChangesAsync();
        return new LoginResponse
{
    AccessToken = token,
    RefreshToken = refreshTokenValue
};

    }
    
}