using AutoMapper;
using Domain.Entities;
using OrderService.Application.Common;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CreateOrderResponse>> Handle(
        Guid userId,
        CreateOrderRequest request)
    {
        const decimal productPrice = 100;

        decimal totalPrice = request.Quantity * productPrice;

        var order = new Order(
            request.CustomerName,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.ProductID,
            request.Quantity,
            totalPrice,
            userId);

        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<CreateOrderResponse>(order);

        return Result<CreateOrderResponse>.Success(
            response,
            "Siparişiniz oluşturuldu.");
    }
}