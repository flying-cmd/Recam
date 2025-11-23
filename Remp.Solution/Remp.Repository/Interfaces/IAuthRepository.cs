using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IAuthRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task CreateAgentAsync(User user, Agent agent, string password, string role);
}
