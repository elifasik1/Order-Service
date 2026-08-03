

using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
         _context = context;
    }

    public async Task AddAsync(User newUser)
    {
        await _context.Users.AddAsync(newUser);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _context.Users
        .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}