using Microsoft.AspNetCore.Identity;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public AuthRepository(AppDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task CreateAgentAsync(User user, Agent agent, string password, string role)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await _userManager.CreateAsync(user, password);

            await _userManager.AddToRoleAsync(user, role);

            _dbContext.Agents.Add(agent);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
