using AutoMapper;
using Moq;
using OrderService.Application.Features.Orders.CreateOrder;
using OrderService.Application.Interfaces;

namespace OrderService.UnitTests;

public class CreateOrderHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateOrder()
    {
        // Arrange
        var repositoryMock = new Mock<IOrderRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();

        mapperMock
            .Setup(x => x.Map<CreateOrderResponse>(
                It.IsAny<Domain.Entities.Order>()))
            .Returns((Domain.Entities.Order order) => new CreateOrderResponse
            {
                TotalPrice = order.TotalPrice,
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status
            });

        var handler = new CreateOrderHandler(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            mapperMock.Object);

        var userId = Guid.NewGuid();

        var request = new CreateOrderRequest
        {
            CustomerName = "Test Kullanıcı",
            Email = "test@test.com",
            PhoneNumber = "05551112233",
            Address = "Antakya Hatay",
            ProductID = 2,
            Quantity = 2
        };

        // Act
        var result = await handler.Handle(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
       Assert.NotNull(result.Data);
Assert.Equal(200, result.Data!.TotalPrice);

        repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Entities.Order>()),
            Times.Once);

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
public async Task Handle_ValidRequest_ShouldAssignCorrectUserId()
{
    // Arrange
    var repositoryMock = new Mock<IOrderRepository>();
    var unitOfWorkMock = new Mock<IUnitOfWork>();
    var mapperMock = new Mock<IMapper>();

    mapperMock
        .Setup(x => x.Map<CreateOrderResponse>(
            It.IsAny<Domain.Entities.Order>()))
        .Returns((Domain.Entities.Order order) => new CreateOrderResponse
        {
            TotalPrice = order.TotalPrice,
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            Status = order.Status
        });

    Domain.Entities.Order? createdOrder = null;

    repositoryMock
        .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Order>()))
        .Callback<Domain.Entities.Order>(order =>
        {
            createdOrder = order;
        });

    var handler = new CreateOrderHandler(
        repositoryMock.Object,
        unitOfWorkMock.Object,
        mapperMock.Object);

    var userId = Guid.NewGuid();

    var request = new CreateOrderRequest
    {
        CustomerName = "Test Kullanıcı",
        Email = "test@test.com",
        PhoneNumber = "05551112233",
        Address = "Antakya Hatay",
        ProductID = 2,
        Quantity = 2
    };

    // Act
    await handler.Handle(userId, request);

    // Assert
    Assert.NotNull(createdOrder);
    Assert.Equal(userId, createdOrder!.UserId);
}
[Fact]
public async Task Handle_WhenRepositoryFails_ShouldThrowException()
{
    // Arrange
    var repositoryMock = new Mock<IOrderRepository>();
    var unitOfWorkMock = new Mock<IUnitOfWork>();
    var mapperMock = new Mock<IMapper>();

    repositoryMock
        .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Order>()))
        .ThrowsAsync(new Exception("Repository error"));

    var handler = new CreateOrderHandler(
        repositoryMock.Object,
        unitOfWorkMock.Object,
        mapperMock.Object);

    var userId = Guid.NewGuid();

    var request = new CreateOrderRequest
    {
        CustomerName = "Test Kullanıcı",
        Email = "test@test.com",
        PhoneNumber = "05551112233",
        Address = "Antakya Hatay",
        ProductID = 2,
        Quantity = 2
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(
        () => handler.Handle(userId, request));

    Assert.Equal("Repository error", exception.Message);

    unitOfWorkMock.Verify(
        x => x.SaveChangesAsync(),
        Times.Never);
}
}
