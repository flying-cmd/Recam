using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAgentToPhotographyCompanyAsync(string agentId, string photographyCompanyId)
    {
        await _context.AgentPhotographyCompanies
            .AddAsync(new AgentPhotographyCompany() { AgentId = agentId, PhotographyCompanyId = photographyCompanyId });

        await _context.SaveChangesAsync();
    }

    public async Task<Agent?> FindAgentByIdAsync(string agentId)
    {
        return await _context.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == agentId);
    }

    public async Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        return await _context.Agents
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(a => a.AgentFirstName)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Agents.AsNoTracking().CountAsync();
    }

    public async Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId)
    {
        return await _context.AgentListingCases
            .Where(alc => alc.AgentId == currentUserId)
            .Select(alc => alc.ListingCaseId)
            .ToListAsync();
    }

    public async Task<bool> IsAgentAddedToPhotographyCompanyAsync(string agentId, string photographyCompanyId)
    {
        return await _context.AgentPhotographyCompanies
            .AnyAsync(apc => apc.AgentId == agentId && apc.PhotographyCompanyId == photographyCompanyId);
    }
}
