using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace OrderService.Infrastructure.Repositories;



public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
         _context = context;
    }
    public async Task AddAsync(Order order)
{
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
}

    public async Task<List<Order>> GetAllAsync()
{
    return await _context.Orders.ToListAsync();
}
}