using Domain.Entities;
using OrderService.Application.Interfaces;


public class CreateOrderHandler
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request)
    {
        const decimal productPrice = 100;
        decimal totalPrice =request.Quantity * productPrice;
        
    
      var order = new Order(
    request.CustomerName,
    request.Email,
    request.PhoneNumber,
    request.Address,
    request.ProductID,
    request.Quantity,
    totalPrice);


            
         await _orderRepository.AddAsync(order);  // ← Yeni eklediğimiz satır    
         await _orderRepository.SaveChangesAsync();   // <-- BUNU EKLE
        return new CreateOrderResponse
        {
             
             TotalPrice = order.TotalPrice,
             Id = order.Id,
             Message = "Siparişiniz oluşturuldu.",
            CreatedAt = order.CreatedAt,
             Status = order.Status
        };
       
        
    }
     private readonly IOrderRepository _orderRepository;

public CreateOrderHandler(IOrderRepository orderRepository)
{
    _orderRepository = orderRepository;
}
}