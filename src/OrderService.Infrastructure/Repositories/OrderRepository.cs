using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context)
        : base(context)
    {
    }



   public async Task<List<Order>> GetPagedAsync(int page, int pageSize)
{
    return await _context.Orders
        .Where(x => !x.IsDeleted)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}

    public async Task<int> CountAsync()
{
    return await _context.Orders
        .CountAsync(x => !x.IsDeleted);
}
public void SetOriginalVersion(Order order, uint version)
{
    _context.Entry(order)
        .Property(x => x.Version)
        .OriginalValue = version;
}
}