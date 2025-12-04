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
    private readonly IBlobStorageService _blobStorageService;

    public AuthService(IAuthRepository authRepository, IJwtTokenService jwtTokenService, IBlobStorageService blobStorageService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _blobStorageService = blobStorageService;
    }

    public async Task<string> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _authRepository.FindByEmailAsync(loginRequest.Email);
        
        if (user is null)
        {
            UserActivityLog.LogLogin(
                email: loginRequest.Email,
                userId: null,
                description: "User failed to login because email is not found"
            );
            throw new UnauthorizedException(message: "Email is not found", title: "Email is incorrect");
        }

        var passwordCheck = await _authRepository.CheckPasswordAsync(user, loginRequest.Password);

        if (!passwordCheck)
        {
            UserActivityLog.LogLogin(
                email: loginRequest.Email,
                userId: user.Id.ToString(),
                description: "User try to login but password is incorrect"
            );
            throw new UnauthorizedException(message: "Password is incorrect", title: "Password is incorrect");
        }

        var token = await _jwtTokenService.CreateTokenAsync(user);

        UserActivityLog.LogLogin(
            email: loginRequest.Email,
            userId: user.Id.ToString(),
            description: "User logged in"
        );
        return token;
    }

    public async Task<string> RegisterAsync(RegisterRequestDto registerUser)
    {
        // Check if the email already exists
        var emailExists = await _authRepository.FindByEmailAsync(registerUser.Email);
        
        if (emailExists != null)
        {
            UserActivityLog.LogRegister(
                email: registerUser.Email,
                userId: null,
                description: "User failed to register because email already exists"
            );
            throw new ArgumentErrorException(message: "Email already exists", title: "Email already exists");
        }

        // Upload avatar to blob storage
        var avatarUrl = await _blobStorageService.UploadFileAsync(registerUser.Avatar);

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
            AvataUrl = avatarUrl
        };

        // Create Agent
        await _authRepository.CreateAgentAsync(user, agent, registerUser.Password, RoleNames.Agent);
        
        UserActivityLog.LogRegister(
            email: registerUser.Email,
            userId: user.Id.ToString(),
            description: $"User registered and is given {RoleNames.Agent} role"
        );
        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }
}
