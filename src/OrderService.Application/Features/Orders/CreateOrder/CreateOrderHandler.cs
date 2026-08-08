using Domain.Entities;
using OrderService.Application.Interfaces;


public class CreateOrderHandler
{
    public async Task<CreateOrderResponse> Handle(Guid userId, CreateOrderRequest request)
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
    totalPrice,
    userId);


         await _orderRepository.AddAsync(order);  // ← Yeni eklediğimiz satır    
         await _unitOfWork.SaveChangesAsync();  // <-- BUNU EKLE
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
     private readonly IUnitOfWork _unitOfWork;


public CreateOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
{
    _orderRepository = orderRepository;
    _unitOfWork = unitOfWork;
}
}