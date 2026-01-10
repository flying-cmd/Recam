using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public async Task CreateAgentAsync(User user, PhotographyCompany photographyCompany, string password, string role)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var createdUser = await _userManager.CreateAsync(user, password);
            if (!createdUser.Succeeded)
            {
                await transaction.RollbackAsync();

                var errors = string.Join("; ", createdUser.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();

                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to add role to user: {errors}");
            }

            _dbContext.PhotographyCompanies.Add(photographyCompany);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
