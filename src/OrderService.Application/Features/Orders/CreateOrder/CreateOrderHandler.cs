using Domain.Entities;
using OrderService.Application.Interfaces;


public class CreateOrderHandler
{
    public CreateOrderResponse Handle(CreateOrderRequest request)
    {
        const decimal productPrice = 100;
        decimal totalPrice =request.Quantity * productPrice;
        
    
        var order = new Order(
            request.CustomerName , totalPrice);
            
         _orderRepository.Add(order);   // ← Yeni eklediğimiz satır    
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