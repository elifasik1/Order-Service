using Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(User user);


}