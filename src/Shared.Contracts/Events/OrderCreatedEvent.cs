namespace Shared.Contracts.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid UserId,
    string CustomerName,
    decimal TotalPrice,
    DateTime CreatedAt
);