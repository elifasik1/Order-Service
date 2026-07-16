public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;// Kullanıcıdan alınacak

    public int PhoneNumber { get; set; } // Kullanıcıdan alınacak

    public string Address { get; set; } = string.Empty; // Kullanıcıdan alınacak

    public int ProductID { get; set; } // Kullanıcıdan alınacak
    public int Quantity { get; set; } // Kullanıcıdan alınacak


}