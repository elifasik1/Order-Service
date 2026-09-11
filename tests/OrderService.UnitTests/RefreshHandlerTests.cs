using Moq;
using OrderService.Application.Interfaces;

namespace OrderService.UnitTests;

public class RefreshHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    private RefreshHandler CreateHandler()
    {
        return new RefreshHandler(
            _refreshTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ShouldThrowUnauthorized()
    {
        // Arrange
        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.RefreshToken?)null);

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "invalid-token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }

    [Fact]
    public async Task Handle_RevokedRefreshToken_ShouldThrowUnauthorized()
    {
        // Arrange
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "revoked-token",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = DateTime.UtcNow
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("revoked-token"))
            .ReturnsAsync(refreshToken);

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "revoked-token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ShouldThrowUnauthorized()
    {
        // Arrange
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-token",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            RevokedAt = null
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("expired-token"))
            .ReturnsAsync(refreshToken);

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "expired-token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = userId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = null
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("valid-token"))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "valid-token"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ShouldGenerateNewTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new Domain.Entities.User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        };

        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "old-token",
            UserId = userId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = null
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("old-token"))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(x => x.GenerateAccessTokenAsync(user))
            .ReturnsAsync("new-access-token");

        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "old-token"
        };

        // Act
        var result = await handler.Handle(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);

        Assert.NotNull(refreshToken.RevokedAt);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(refreshToken),
            Times.Once);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.RefreshToken>(
                token => token.Token == "new-refresh-token"
                         && token.UserId == userId)),
            Times.Once);

        _refreshTokenRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        _jwtServiceMock.Verify(
            x => x.GenerateAccessTokenAsync(user),
            Times.Once);

        _jwtServiceMock.Verify(
            x => x.GenerateRefreshToken(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsReused_ShouldThrowUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new Domain.Entities.User
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        };

        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "old-token",
            UserId = userId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = null
        };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync("old-token"))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(x => x.GenerateAccessTokenAsync(user))
            .ReturnsAsync("new-access-token");

        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = "old-token"
        };

        // Act
        await handler.Handle(request);

        // Assert
        Assert.NotNull(refreshToken.RevokedAt);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }

    [Fact]
    public async Task Handle_EmptyRefreshToken_ShouldThrowUnauthorized()
    {
        // Arrange
        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(""))
            .ReturnsAsync((Domain.Entities.RefreshToken?)null);

        var handler = CreateHandler();

        var request = new RefreshRequest
        {
            RefreshToken = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));
    }
}