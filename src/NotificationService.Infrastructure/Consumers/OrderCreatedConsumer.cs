using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Data;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly NotificationDbContext _dbContext;

    public OrderCreatedConsumer(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(
        ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification(
            message.UserId,
            message.OrderId,
            "Siparişiniz başarıyla oluşturuldu.");

        _dbContext.Notifications.Add(notification);

        await _dbContext.SaveChangesAsync();
    }
}