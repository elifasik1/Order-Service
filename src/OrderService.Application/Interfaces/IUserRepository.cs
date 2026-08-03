using Domain.Entities;
namespace OrderService.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User newUser);
    Task <User?> FindByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<User?> FindByIdAsync(Guid id);
}
