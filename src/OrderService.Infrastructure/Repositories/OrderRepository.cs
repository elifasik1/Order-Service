using Domain.Entities;
using OrderService.Application.Interfaces;
namespace OrderService.Infrastructure.Repositories;



public class OrderRepository : IOrderRepository
{
    public void Add(Order order)
    {
        // TODO: Save to database
    }
}