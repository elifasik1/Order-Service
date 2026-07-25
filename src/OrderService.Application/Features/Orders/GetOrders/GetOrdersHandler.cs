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
    public GetOrdersResponse Handle()
    {
        var orders = _orderRepository.GetAll();
        return new GetOrdersResponse
        {
            Orders = orders
        };
    
    }
}


