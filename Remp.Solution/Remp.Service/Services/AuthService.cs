using Remp.Models.Entities;
using Remp.Common.Exceptions;
using Remp.Service.Interfaces;
using Remp.Service.DTOs;
using Remp.Models.Constants;
using Remp.Repository.Interfaces;

namespace Remp.Service.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IAuthRepository authRepository, IJwtTokenService jwtTokenService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _authRepository.FindByEmailAsync(loginRequest.Email);
        
        if (user is null)
        {
            UserActivityLogService.LogLogin(
                email: loginRequest.Email,
                userId: null,
                description: "User failed to login because email is not found"
            );
            throw new UnauthorizedException(message: "Email is not found", title: "Email is incorrect");
        }

        var passwordCheck = await _authRepository.CheckPasswordAsync(user, loginRequest.Password);

        if (!passwordCheck)
        {
            UserActivityLogService.LogLogin(
                email: loginRequest.Email,
                userId: user.Id.ToString(),
                description: "User try to login but password is incorrect"
            );
            throw new UnauthorizedException(message: "Password is incorrect", title: "Password is incorrect");
        }

        var token = await _jwtTokenService.CreateTokenAsync(user);

        UserActivityLogService.LogLogin(
            email: loginRequest.Email,
            userId: user.Id.ToString(),
            description: "User logged in"
        );
        return token;
    }

    public async Task<string> RegisterAsync(RegisterUserDto registerUser)
    {
        var emailExists = await _authRepository.FindByEmailAsync(registerUser.Email);
        
        if (emailExists != null)
        {
            UserActivityLogService.LogRegister(
                email: registerUser.Email,
                userId: null,
                description: "User failed to register because email already exists"
            );
            throw new RegisterException(message: "Email already exists", title: "Email already exists");
        }

        var user = new User()
        {
            Email = registerUser.Email,
            UserName = registerUser.Email
        };
        
        var agent = new Agent()
        {
            Id = user.Id,
            AgentFirstName = registerUser.FirstName,
            AgentLastName = registerUser.LastName,
            CompanyName = registerUser.CompanyName,
            AvataUrl = registerUser.AvatarUrl,
        };

        // Create Agent
        try
        {
            await _authRepository.CreateAgentAsync(user, agent, registerUser.Password, RoleNames.Agent);
        }
        catch (Exception e)
        {
            UserActivityLogService.LogRegister(
                email: registerUser.Email,
                userId: null,
                description: $"User failed to register with errors: {e.Message}"
            );
            throw new RegisterException(message: e.Message, title: "User registration failed");
        }
        
        UserActivityLogService.LogRegister(
            email: registerUser.Email,
            userId: user.Id.ToString(),
            description: $"User registered and is given {RoleNames.Agent} role"
        );
        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }
}
