using Microsoft.AspNetCore.Identity;
using Remp.Models.Entities;
using Remp.Common.Exceptions;
using Remp.Service.Interfaces;
using Remp.Service.DTOs;
using Remp.DataAccess.Data;
using Remp.Models.Constants;

namespace Remp.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly AppDbContext _dbContext;

    public AuthService(UserManager<User> userManager, IJwtTokenService jwtTokenService, AppDbContext dbContext)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
    }

    public async Task<string> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        
        if (user is null)
        {
            UserActivityLogService.LogLogin(
                email: loginRequest.Email,
                userId: null,
                description: "User failed to login because email is not found"
            );
            throw new UnauthorizedException(message: "Email is not found", title: "Email is incorrect");
        }

        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

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
        var emailExists = await _userManager.FindByEmailAsync(registerUser.Email);
        
        if (emailExists != null)
        {
            UserActivityLogService.LogRegister(
                email: registerUser.Email,
                userId: null,
                description: "User failed to register because email already exists"
            );
            throw new RegisterException(message: "Email already exists", title: "Email already exists");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
    
        var user = new User()
        {
            Email = registerUser.Email,
            UserName = registerUser.Email
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);
        if (!result.Succeeded)
        {
            string message = string.Join("; ", result.Errors.Select(e => e.Description));
            UserActivityLogService.LogRegister(
                email: registerUser.Email,
                userId: null,
                description: $"User failed to register with errors: {message}"
            );
            throw new RegisterException(message: message, title: "User registration failed");
        }

        await _userManager.AddToRoleAsync(user, RoleNames.Agent);
        
        var agent = new Agent()
        {
            Id = user.Id,
            AgentFirstName = registerUser.FirstName,
            AgentLastName = registerUser.LastName,
            CompanyName = registerUser.CompanyName,
            AvataUrl = registerUser.AvatarUrl,
        };

        _dbContext.Agents.Add(agent);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        
        UserActivityLogService.LogRegister(
            email: registerUser.Email,
            userId: user.Id.ToString(),
            description: $"User registered and is given {RoleNames.Agent} role"
        );
        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }
}
