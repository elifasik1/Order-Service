using Domain.Entities;
using OrderService.Application.Interfaces;

public class GetOrdersHandler
{
    private readonly IOrderRepository _orderRepository;

    // Constructor
    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
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


