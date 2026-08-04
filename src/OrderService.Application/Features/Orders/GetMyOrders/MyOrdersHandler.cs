using Domain.Entities;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Orders.GetMyOrders;

public class MyOrdersHandler
{
    private readonly IOrderRepository _orderRepository;

    public MyOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Order>> Handle(Guid userId)
    {
        return await _orderRepository.GetByUserIdAsync(userId);
    }
}