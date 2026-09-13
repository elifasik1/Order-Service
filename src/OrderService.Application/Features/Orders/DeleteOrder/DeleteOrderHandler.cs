using OrderService.Application.Common;
using OrderService.Application.Interfaces;

public class DeleteOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderCacheService _orderCacheService;

    public DeleteOrderHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IOrderCacheService orderCacheService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _orderCacheService = orderCacheService;
    }

    public async Task<Result<Guid>> Handle(Guid id)
    {
        var order = await _orderRepository.FindByIdAsync(id);

        if (order == null)
        {
            return Result<Guid>.Failure("Sipariş bulunamadı.");
        }

        order.SoftDelete();

        await _unitOfWork.SaveChangesAsync();

        await _orderCacheService.InvalidateOrdersCacheAsync();

        return Result<Guid>.Success(
            order.Id,
            "Sipariş başarıyla silindi."
        );
    }
}