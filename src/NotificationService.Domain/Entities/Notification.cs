namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid OrderId { get; private set; }
public string Message { get; private set; } = string.Empty;    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid userId,
        Guid orderId,
        string message)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OrderId = orderId;
        Message = message;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}