using Domain.Enums;
namespace Domain.Entities
{
    public class Order
    {
       public Order(
    string customerName,
    string email,
    string phoneNumber,
    string address,
    int productId,
    int quantity,
    decimal totalPrice)
{
    Id = Guid.NewGuid();
    CustomerName = customerName;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
    ProductId = productId;
    Quantity = quantity;
    TotalPrice = totalPrice;
    Status = OrderStatus.Pending;
    CreatedAt = DateTime.UtcNow;
}
public void Update(
    string customerName,
    string email,
    string phoneNumber,
    string address,
    int productId,
    int quantity,
    decimal totalPrice)
{
    CustomerName = customerName;
    Email = email;
    PhoneNumber = phoneNumber;
    Address = address;
    ProductId = productId;
    Quantity = quantity;
    TotalPrice = totalPrice;
}
        public Guid Id { get; private set; }
        public string CustomerName { get; private set; } // Kullanıcıdan alınacak
        public decimal TotalPrice { get; private set; }  // Sistem tarafından hesaplanacak
        public OrderStatus Status { get; private set; }  // Sistem tarafından yönetilecek
        public DateTime CreatedAt { get; private set; }  // Sistem tarafından atanacak

        public string Email { get; private set; }
public string PhoneNumber { get; private set; }
public string Address { get; private set; }
public int ProductId { get; private set; }
public int Quantity { get; private set; }
        // Status güncellemesi için Domain metodu (Business Logic)
        public void UpdateStatus(OrderStatus newStatus)
        {
            // İstersen buraya "Pending'den direkt Delivered'a geçemez" gibi kurallar ekleyebilirsin
            Status = newStatus;
        }
    }
}