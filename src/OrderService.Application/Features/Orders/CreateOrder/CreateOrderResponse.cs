using Domain.Enums;

public class CreateOrderResponse
{
    
    public decimal TotalPrice { get; set; }
    public Guid Id { get; set; }

    public string Message { get; set; } = string.Empty;// Kullanıcıya gösterilecek mesaj

    public DateTime CreatedAt { get; set; } // Siparişin oluşturulma zamanı

    public OrderStatus Status { get; set; }

    
}
