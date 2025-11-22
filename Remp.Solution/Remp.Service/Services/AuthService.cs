using Microsoft.AspNetCore.Identity;
using Remp.Models.Entities;
using Remp.Common.Exceptions;
using Remp.Service.Interfaces;
using Remp.Service.DTOs;
using Remp.DataAccess.Data;

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
            throw new UnauthorizedException(message: "Email is not found", title: "Email is incorrect");
        }

        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!passwordCheck)
        {
            throw new UnauthorizedException(message: "Password is incorrect", title: "Password is incorrect");
        }

        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }

    public async Task<string> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var emailExists = await _userManager.FindByEmailAsync(registerRequest.Email);
        
        if (emailExists != null)
        {
            throw new RegisterException(message: "Email already exists", title: "Email already exists");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
    
        var user = new User()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Email
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
        {
            throw new RegisterException(message: string.Join("; ", result.Errors.Select(e => e.Description)), title: "User registration failed");
        }

        await _userManager.AddToRoleAsync(user, "Agent");
        
        var agent = new Agent()
        {
            Id = user.Id,
            AgentFirstName = registerRequest.FirstName,
            AgentLastName = registerRequest.LastName,
            CompanyName = registerRequest.CompanyName,
            AvataUrl = registerRequest.AvatarUrl,
        };

        _dbContext.Agents.Add(agent);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        
        var token = await _jwtTokenService.CreateTokenAsync(user);
        return token;
    }
}
