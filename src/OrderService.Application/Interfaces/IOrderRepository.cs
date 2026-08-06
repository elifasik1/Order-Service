
using Domain.Entities;
namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    
    Task AddAsync(Order order);
    Task<List<Order>> GetAllAsync();

    Task<Order?> FindByIdAsync(Guid orderId);
    Task SaveChangesAsync();
    public void Delete(Order order);
    Task<List<Order>> GetByUserIdAsync(Guid userId);
    Task<List<Order>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();

}
