using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Data;
using Shared.Contracts.Events;
using Moq;
namespace NotificationService.UnitTests;

public class OrderCreatedConsumerTests
{
    [Fact]
    public async Task Consume_ShouldCreateNotification()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new NotificationDbContext(options);

        var consumer = new OrderCreatedConsumer(dbContext);

        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var message = new OrderCreatedEvent(
            orderId,
            userId,
            "Elif",
            500,
            DateTime.UtcNow);

        var context = Mock.Of<ConsumeContext<OrderCreatedEvent>>(
            x => x.Message == message);

        // Act
        await consumer.Consume(context);

        // Assert
        var notification = await dbContext.Notifications.SingleAsync();

        Assert.Equal(orderId, notification.OrderId);
        Assert.Equal(userId, notification.UserId);
        Assert.Equal("Siparişiniz başarıyla oluşturuldu.", notification.Message);
        Assert.False(notification.IsRead);
    }
}