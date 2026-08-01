using Domain.Entities;
namespace OrderService.Application.Interfaces;

public interface IUserRepository
{
    
    Task <User?> FindByEmailAsync(string email);
    

}
