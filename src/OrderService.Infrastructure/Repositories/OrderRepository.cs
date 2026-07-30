using Domain.Entities;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
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
   await _context.Orders.AddAsync(order);
}

    public void Delete(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task<List<Order>> GetAllAsync()
{
    return await _context.Orders.ToListAsync();
}

    public async Task SaveChangesAsync()
    {
         await _context.SaveChangesAsync();
    }

    async Task<Order?> IOrderRepository.FindByIdAsync(Guid orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }
}