using OrderService.Application.Common;
using OrderService.Application.Interfaces;

public class UpdateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateOrderResponse>> Handle(
        Guid id,
        UpdateOrderRequest request)
    {
        var order = await _orderRepository.FindByIdAsync(id);

        if (order == null)
        {
            return Result<UpdateOrderResponse>.Failure(
                "Sipariş bulunamadı.");
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

        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateOrderResponse
        {
            TotalPrice = order.TotalPrice,
            Id = order.Id,
            Status = order.Status
        };

        return Result<UpdateOrderResponse>.Success(
            response,
            "Sipariş başarıyla güncellendi.");
    }
}