using OrderService.Application.Interfaces;
namespace OrderService.Application.Features.Auth.Login;
public class LoginHandler
{
    private readonly IUserRepository _userRepository;

    private readonly IJwtService _jwtService;
    public LoginHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

   public async Task<LoginResponse> Handle(LoginRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        if (user.PasswordHash != request.Password)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = await _jwtService.GenerateAccessTokenAsync(user);

        return new LoginResponse
        {
            AccessToken = token
        };
       
    }


    
}