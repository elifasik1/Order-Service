using OrderService.Application.Interfaces;

public class DeleteOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    public DeleteOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public async Task<DeleteOrderResponse> Handle(Guid id)
    {
        var order = await _orderRepository.FindByIdAsync(id);
        if (order == null)
        {
            return new DeleteOrderResponse
            {
                 Success = false,
    Message = "Sipariş bulunamadı."
            };
        }
        _orderRepository.Delete(order);
        await _orderRepository.SaveChangesAsync();
        return new DeleteOrderResponse
        {
             Success = true,
    Id = order.Id,
    Message = "Sipariş başarıyla silindi."
        };
    }
}