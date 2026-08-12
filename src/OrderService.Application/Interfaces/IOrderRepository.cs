using Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetPagedAsync(int page, int pageSize);

    Task<int> CountAsync();
    void SetOriginalVersion(Order order, uint version);
}