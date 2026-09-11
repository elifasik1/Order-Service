using Moq;
using OrderService.Application.Interfaces;

namespace OrderService.UnitTests;

public class RefreshHandlerTests
{
    [Fact]
    public async Task Handle_InvalidRefreshToken_ShouldThrowUnauthorized()
    {
        // Arrange
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtServiceMock = new Mock<IJwtService>();

        refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.RefreshToken?)null);

        var handler = new RefreshHandler(
            refreshTokenRepositoryMock.Object,
            userRepositoryMock.Object,
            jwtServiceMock.Object);

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
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

    var refreshToken = new Domain.Entities.RefreshToken
    {
        Id = Guid.NewGuid(),
        Token = "revoked-token",
        UserId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        ExpiresAt = DateTime.UtcNow.AddDays(1),
        RevokedAt = DateTime.UtcNow
    };

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync("revoked-token"))
        .ReturnsAsync(refreshToken);

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

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
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

    var refreshToken = new Domain.Entities.RefreshToken
    {
        Id = Guid.NewGuid(),
        Token = "expired-token",
        UserId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow.AddDays(-2),
        ExpiresAt = DateTime.UtcNow.AddDays(-1),
        RevokedAt = null
    };

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync("expired-token"))
        .ReturnsAsync(refreshToken);

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

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
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

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

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync("valid-token"))
        .ReturnsAsync(refreshToken);

    userRepositoryMock
        .Setup(x => x.FindByIdAsync(userId))
        .ReturnsAsync((Domain.Entities.User?)null);

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

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
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

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

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync("old-token"))
        .ReturnsAsync(refreshToken);

    userRepositoryMock
        .Setup(x => x.FindByIdAsync(userId))
        .ReturnsAsync(user);

    jwtServiceMock
        .Setup(x => x.GenerateAccessTokenAsync(user))
        .ReturnsAsync("new-access-token");

    jwtServiceMock
        .Setup(x => x.GenerateRefreshToken())
        .Returns("new-refresh-token");

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

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

    refreshTokenRepositoryMock.Verify(
        x => x.UpdateAsync(refreshToken),
        Times.Once);

    refreshTokenRepositoryMock.Verify(
        x => x.AddAsync(It.Is<Domain.Entities.RefreshToken>(
            token => token.Token == "new-refresh-token"
                    && token.UserId == userId)),
        Times.Once);

    refreshTokenRepositoryMock.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);

    jwtServiceMock.Verify(
        x => x.GenerateAccessTokenAsync(user),
        Times.Once);

    jwtServiceMock.Verify(
        x => x.GenerateRefreshToken(),
        Times.Once);
}
[Fact]
public async Task Handle_WhenRefreshTokenIsReused_ShouldThrowUnauthorized()
{
    // Arrange
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

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

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync("old-token"))
        .ReturnsAsync(refreshToken);

    userRepositoryMock
        .Setup(x => x.FindByIdAsync(userId))
        .ReturnsAsync(user);

    jwtServiceMock
        .Setup(x => x.GenerateAccessTokenAsync(user))
        .ReturnsAsync("new-access-token");

    jwtServiceMock
        .Setup(x => x.GenerateRefreshToken())
        .Returns("new-refresh-token");

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

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
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();

    refreshTokenRepositoryMock
        .Setup(x => x.GetByTokenAsync(""))
        .ReturnsAsync((Domain.Entities.RefreshToken?)null);

    var handler = new RefreshHandler(
        refreshTokenRepositoryMock.Object,
        userRepositoryMock.Object,
        jwtServiceMock.Object);

    var request = new RefreshRequest
    {
        RefreshToken = ""
    };

    // Act & Assert
    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => handler.Handle(request));
}
}