using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Remp.Service.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public JwtTokenService(UserManager<User> userManager, IConfiguration configuration, IUserRepository userRepository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<string> CreateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Assign scopes based on user roles
        claims.Add(new Claim("scopes", roles.Contains(RoleNames.PhotographyCompany) ? RoleNames.PhotographyCompany : RoleNames.Agent));

        // Add agent first name and last name if the user is an agent
        if (roles.Contains(RoleNames.Agent))
        {
            var agent = await _userRepository.FindAgentByIdAsync(user.Id);
            claims.Add(new Claim("agentFirstName", agent?.AgentFirstName ?? string.Empty));
            claims.Add(new Claim("agentLastName", agent?.AgentLastName ?? string.Empty));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:DurationInMinutes"]!)),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
