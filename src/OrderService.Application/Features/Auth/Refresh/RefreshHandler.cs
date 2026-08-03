

using Domain.Entities;
using OrderService.Application.Interfaces;

public class RefreshHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RefreshHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IJwtService jwtService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<RefreshResponse> Handle(RefreshRequest request)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

if (existingRefreshToken == null)
{
    throw new UnauthorizedAccessException("Invalid refresh token.");
}
if (existingRefreshToken.RevokedAt != null)
{
    throw new UnauthorizedAccessException("Refresh token has been revoked.");
}

if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
{
    throw new UnauthorizedAccessException("Refresh token expired.");
}
var user = await _userRepository.FindByIdAsync(existingRefreshToken.UserId);

if (user == null)
{
    throw new UnauthorizedAccessException("User not found.");
}
var newAccessToken = await _jwtService.GenerateAccessTokenAsync(user);
existingRefreshToken.RevokedAt = DateTime.UtcNow;
await _refreshTokenRepository.UpdateAsync(existingRefreshToken);
var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
var newRefreshToken = new RefreshToken
{
    Id = Guid.NewGuid(),
    Token = newRefreshTokenValue,
    UserId = user.Id,
    User = user,
    CreatedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddDays(7)
};
await _refreshTokenRepository.AddAsync(newRefreshToken);
await _refreshTokenRepository.SaveChangesAsync();
return new RefreshResponse
{
    AccessToken = newAccessToken,
    RefreshToken = newRefreshTokenValue
};
    }
    
}