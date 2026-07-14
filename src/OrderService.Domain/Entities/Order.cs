using Domain.Enums;
namespace Domain.Entities
{
    public class Order
    {
        public Order(string customerName, decimal totalPrice)
        {
            Id = Guid.NewGuid();
            CustomerName = customerName;
            TotalPrice = totalPrice;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; private set; }
        public string CustomerName { get; private set; } // Kullanıcıdan alınacak
        public decimal TotalPrice { get; private set; }  // Sistem tarafından hesaplanacak
        public OrderStatus Status { get; private set; }  // Sistem tarafından yönetilecek
        public DateTime CreatedAt { get; private set; }  // Sistem tarafından atanacak

        // Status güncellemesi için Domain metodu (Business Logic)
        public void UpdateStatus(OrderStatus newStatus)
        {
            // İstersen buraya "Pending'den direkt Delivered'a geçemez" gibi kurallar ekleyebilirsin
            Status = newStatus;
        }
    }
}