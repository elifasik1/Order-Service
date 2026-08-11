using Domain.Enums;

public class CreateOrderResponse
{
    public decimal TotalPrice { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
}