using Domain.Entities;
using OrderService.Application.Interfaces;
namespace OrderService.Infrastructure.Repositories;



public class OrderRepository : IOrderRepository
{
    private List<Order> _orders = new();
    public void Add(Order order)
    {
        _orders.Add(order);
    }

    public List<Order> GetAll()
    {
        return _orders;
    }
}