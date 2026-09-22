

using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using OrderService.Application.Interfaces;
namespace OrderService.Application.Features.Auth.Register;
public class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IJwtService _jwtService;
    public RegisterHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user != null)
        {
            throw new UnauthorizedAccessException("Bu mail adresi zaten kayıtlı.");
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = string.Empty,
            UserRole = Domain.Enums.UserRole.User,
            UserStatus = Domain.Enums.UserStatus.Active,
            CreatedAt = DateTime.UtcNow
            
        };

           newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();   // <-- BUNU EKLE


        var token = await _jwtService.GenerateAccessTokenAsync(newUser);

        return new RegisterResponse
        {
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            AccessToken = token
        };
       
    }
}