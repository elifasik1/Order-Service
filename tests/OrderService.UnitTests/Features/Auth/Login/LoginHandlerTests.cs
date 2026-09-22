using Moq;
using OrderService.Application.Interfaces;
using OrderService.Application.Features.Auth.Login;
using Microsoft.AspNetCore.Identity;
namespace OrderService.UnitTests;

public class LoginHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_InvalidCredentials_ShouldThrowUnauthorized(
        bool userExists)
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var jwtServiceMock = new Mock<IJwtService>();
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        var passwordHasher = new PasswordHasher<Domain.Entities.User>();

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = userExists
                ? "wrong-password"
                : "any-password"
        };

        if (userExists)
        {
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHasher.HashPassword(null!, "correct-password")
            };

            userRepositoryMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
        }
        else
        {
            userRepositoryMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((Domain.Entities.User?)null);
        }

        var handler = new LoginHandler(
            userRepositoryMock.Object,
            passwordHasher,
            jwtServiceMock.Object,
            refreshTokenRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(request));

        Assert.Equal(
            "Invalid email or password.",
            exception.Message);
    }
    [Fact]
public async Task Handle_ValidCredentials_ShouldReturnTokensAndSaveRefreshToken()
{
    // Arrange
    var userRepositoryMock = new Mock<IUserRepository>();
    var jwtServiceMock = new Mock<IJwtService>();
    var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    var passwordHasher = new PasswordHasher<Domain.Entities.User>();

    var user = new Domain.Entities.User
    {
        Id = Guid.NewGuid(),
        Email = "test@test.com",
        PasswordHash = passwordHasher.HashPassword(null!, "correct-password")
    };

    userRepositoryMock
        .Setup(x => x.FindByEmailAsync(user.Email))
        .ReturnsAsync(user);

    jwtServiceMock
        .Setup(x => x.GenerateAccessTokenAsync(user))
        .ReturnsAsync("access-token");

    jwtServiceMock
        .Setup(x => x.GenerateRefreshToken())
        .Returns("refresh-token");

    var handler = new LoginHandler(
        userRepositoryMock.Object,
        passwordHasher,
        jwtServiceMock.Object,
        refreshTokenRepositoryMock.Object);

    var request = new LoginRequest
    {
        Email = user.Email,
        Password = "correct-password"
    };

    // Act
    var result = await handler.Handle(request);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("access-token", result.AccessToken);
    Assert.Equal("refresh-token", result.RefreshToken);

    refreshTokenRepositoryMock.Verify(
        x => x.AddAsync(It.Is<Domain.Entities.RefreshToken>(
            token => token.Token == "refresh-token"
                && token.UserId == user.Id)),
        Times.Once);

    refreshTokenRepositoryMock.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}
}