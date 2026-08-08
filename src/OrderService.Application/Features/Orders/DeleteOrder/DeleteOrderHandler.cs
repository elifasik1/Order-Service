using OrderService.Application.Interfaces;

public class DeleteOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
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
await _unitOfWork.SaveChangesAsync();
        return new DeleteOrderResponse
        {
             Success = true,
    Id = order.Id,
    Message = "Sipariş başarıyla silindi."
        };
    }
}