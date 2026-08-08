using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
namespace OrderService.Infrastructure.Repositories;



public class OrderRepository : Repository<Order>, IOrderRepository{
    
   public OrderRepository(ApplicationDbContext context)
    : base(context)
{
}



    public async Task SaveChangesAsync()
    {
         await _context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
{
    return await _context.Orders
        .Where(x => x.UserId == userId)
        .ToListAsync();
}

   public async Task<List<Order>> GetPagedAsync(int page, int pageSize)
{
    return await _context.Orders
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
public async Task<int> CountAsync()
{
    return await _context.Orders.CountAsync();
}
}