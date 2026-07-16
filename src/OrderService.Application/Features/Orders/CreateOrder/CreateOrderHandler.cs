using Domain.Entities;


public class CreateOrderHandler
{
    public CreateOrderResponse Handle(CreateOrderRequest request)
    {
        const decimal productPrice = 100;
        decimal totalPrice =request.Quantity * productPrice;
        
    
        var order = new Order(
            request.CustomerName , totalPrice);

        return new CreateOrderResponse
        {
             
             TotalPrice = order.TotalPrice,
             Id = order.Id,
             Message = "Siparişiniz oluşturuldu.",
            CreatedAt = order.CreatedAt,
             Status = order.Status
        };
        


    }
}