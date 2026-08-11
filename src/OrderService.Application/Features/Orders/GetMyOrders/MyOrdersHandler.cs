using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Application.Specifications;

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
    var specification = new OrdersByUserSpecification(userId);

    return await _orderRepository.GetBySpecificationAsync(
        specification);
}
}