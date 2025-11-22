using Remp.Models.Entities;

namespace Remp.Service.Interfaces;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(User user);
}
