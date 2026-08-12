using OrderService.Application.Common;
using OrderService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        _orderRepository.SetOriginalVersion(order, request.Version);

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

        try
{
    await _unitOfWork.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    return Result<UpdateOrderResponse>.Failure(
        "Sipariş başka bir kullanıcı tarafından güncellendi. Lütfen siparişi yenileyip tekrar deneyin.");
}


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