using Domain.Entities;
using OrderService.Application.Interfaces;

public class GetOrdersHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Constructor
    public GetOrdersHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    // Metot
    public async Task<GetOrdersResponse> Handle()
    {
        var orders = await _orderRepository.GetAllAsync();
        return new GetOrdersResponse
        {
            Orders = orders
        };
    
    }
}


