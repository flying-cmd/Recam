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
    private readonly ILoggerService _loggerService;

    public AuthService(
        IAuthRepository authRepository, 
        IJwtTokenService jwtTokenService, 
        IBlobStorageService blobStorageService,
        ILoggerService loggerService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _blobStorageService = blobStorageService;
        _loggerService = loggerService;
    }

    public async Task<string> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _authRepository.FindByEmailAsync(loginRequest.Email);
        
        if (user is null)
        {
            await _loggerService.LogLogin(
                email: loginRequest.Email,
                userId: null,
                error: "Email does not exist"
            );
            throw new UnauthorizedException(message: "Email does not exist", title: "Email does not exist");
        }

        var passwordCheck = await _authRepository.CheckPasswordAsync(user, loginRequest.Password);

        if (!passwordCheck)
        {
            await _loggerService.LogLogin(
                email: loginRequest.Email,
                userId: user.Id.ToString(),
                error: "Password is incorrect"
            );
            throw new UnauthorizedException(message: "Password is incorrect", title: "Password is incorrect");
        }

        var token = await _jwtTokenService.CreateTokenAsync(user);

        await _loggerService.LogLogin(
            email: loginRequest.Email,
            userId: user.Id.ToString()
        );
        return token;
    }

    public async Task<string> RegisterAsync(RegisterRequestDto registerUser)
    {
        // Check if the email already exists
        var emailExists = await _authRepository.FindByEmailAsync(registerUser.Email);
        
        if (emailExists != null)
        {
            await _loggerService.LogRegister(
                email: registerUser.Email,
                userId: null,
                error: "Email already exists"
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
            AvatarUrl = avatarUrl
        };

        // Create Agent
        await _authRepository.CreateAgentAsync(user, agent, registerUser.Password, RoleNames.Agent);
        
        // Log
        await _loggerService.LogRegister(
            email: registerUser.Email,
            userId: user.Id.ToString()
        );

        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }
}
