using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequestDto loginRequest);
    Task<string> RegisterAsync(RegisterUserDto registerUser);
}
