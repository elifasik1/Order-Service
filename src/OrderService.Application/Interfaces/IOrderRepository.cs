using Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task SaveChangesAsync();

    Task<List<Order>> GetByUserIdAsync(Guid userId);

    Task<List<Order>> GetPagedAsync(int page, int pageSize);

    Task<int> CountAsync();
}