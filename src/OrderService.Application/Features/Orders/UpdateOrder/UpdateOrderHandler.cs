
using OrderService.Application.Interfaces;


public class UpdateOrderHandler
{
    private readonly IOrderRepository _orderRepository;

public UpdateOrderHandler(IOrderRepository orderRepository)
{
    _orderRepository = orderRepository;
}
public async Task<UpdateOrderResponse> Handle
(Guid id , UpdateOrderRequest request)
    {
        var order = await _orderRepository.FindByIdAsync(id);
        if (order == null)
        {
            return new UpdateOrderResponse
            {
               Message = "Sipariş bulunamadı."
            };
        }
        const decimal productPrice = 100;
decimal totalPrice = request.Quantity * productPrice;

order.Update(
    request.CustomerName,
    request.Email,
    request.PhoneNumber,
    request.Address,
    request.ProductID,
    request.Quantity,
    totalPrice);
    await _orderRepository.SaveChangesAsync();

    return new UpdateOrderResponse
    {
             TotalPrice = order.TotalPrice,
             Id = order.Id,
             Message = "Sipariş başarıyla güncellendi.",
             Status = order.Status
    };


 }}