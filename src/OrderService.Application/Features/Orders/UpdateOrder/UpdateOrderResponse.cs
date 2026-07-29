
using Domain.Enums;

public class UpdateOrderResponse
{
        public Guid Id { get; set; }

    public decimal TotalPrice { get; set; }

    public OrderStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;
}