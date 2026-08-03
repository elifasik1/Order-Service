

using Domain.Entities;
using OrderService.Application.Interfaces;
namespace OrderService.Application.Features.Auth.Register;
public class RegisterHandler
{
      private readonly IUserRepository _userRepository;

    private readonly IJwtService _jwtService;
    public RegisterHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
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
            PasswordHash = request.Password,
            UserRole = Domain.Enums.UserRole.User,
            UserStatus = Domain.Enums.UserStatus.Active,
            CreatedAt = DateTime.UtcNow
            
             // In a real application, you should hash the password before storing it.
        };

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