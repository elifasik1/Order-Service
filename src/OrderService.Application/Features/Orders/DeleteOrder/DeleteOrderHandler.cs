using OrderService.Application.Common;
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

    public async Task<Result<Guid>> Handle(Guid id)
    {
        var order = await _orderRepository.FindByIdAsync(id);

        if (order == null)
        {
            return Result<Guid>.Failure("Sipariş bulunamadı.");
        }

        _orderRepository.Delete(order);

        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(
            order.Id,
            "Sipariş başarıyla silindi."
        );
    }
}