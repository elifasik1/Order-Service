using Moq;
using OrderService.Application.Interfaces;

namespace OrderService.UnitTests;

public class UpdateOrderHandlerTests
{
    [Fact]
    public async Task Handle_OrderNotFound_ShouldReturnFailure()
    {
        // Arrange
        var repositoryMock = new Mock<IOrderRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Order?)null);

        var handler = new UpdateOrderHandler(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var orderId = Guid.NewGuid();

        var request = new UpdateOrderRequest
        {
            CustomerName = "Test Kullanıcı",
            Email = "test@test.com",
            PhoneNumber = "05551112233",
            Address = "Antakya Hatay",
            ProductID = 2,
            Quantity = 3,
            Version = 811
        };

        // Act
        var result = await handler.Handle(orderId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Sipariş bulunamadı.", result.Message);

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }
    [Fact]
public async Task Handle_ConcurrencyException_ShouldReturnFailure()
{
    // Arrange
    var repositoryMock = new Mock<IOrderRepository>();
    var unitOfWorkMock = new Mock<IUnitOfWork>();

    var orderId = Guid.NewGuid();

    var order = new Domain.Entities.Order(
        "Test Kullanıcı",
        "test@test.com",
        "05551112233",
        "Antakya Hatay",
        2,
        2,
        200,
        Guid.NewGuid());

    repositoryMock
        .Setup(x => x.FindByIdAsync(orderId))
        .ReturnsAsync(order);

    unitOfWorkMock
        .Setup(x => x.SaveChangesAsync())
        .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException());

    var handler = new UpdateOrderHandler(
        repositoryMock.Object,
        unitOfWorkMock.Object);

    var request = new UpdateOrderRequest
    {
        CustomerName = "Concurrency Test",
        Email = "test@test.com",
        PhoneNumber = "05551112233",
        Address = "Antakya Hatay",
        ProductID = 2,
        Quantity = 3,
        Version = 811
    };

    // Act
    var result = await handler.Handle(orderId, request);

    // Assert
    Assert.False(result.IsSuccess);

    Assert.Equal(
        "Sipariş başka bir kullanıcı tarafından güncellendi. Lütfen siparişi yenileyip tekrar deneyin.",
        result.Message);
}
}