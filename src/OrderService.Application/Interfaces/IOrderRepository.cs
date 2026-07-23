
using Domain.Entities;
namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    
    void Add(Order order);
    List<Order> GetAll();
}
