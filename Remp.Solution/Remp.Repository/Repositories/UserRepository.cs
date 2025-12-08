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

    public async Task AddAgentAsync(Agent agent)
    {
        await _context.Agents.AddAsync(agent);
        await _context.SaveChangesAsync();
    }

    public async Task AddAgentToPhotographyCompanyAsync(AgentPhotographyCompany agentPhotographyCompany)
    {
        await _context.AgentPhotographyCompanies
            .AddAsync(agentPhotographyCompany);

        await _context.SaveChangesAsync();
    }

    public async Task<Agent?> FindAgentByIdAsync(string agentId)
    {
        return await _context.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == agentId);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<PhotographyCompany?> FindPhotographyCompanyByIdAsync(string photographyCompanyId)
    {
        return await _context.PhotographyCompanies
            .AsNoTracking()
            .FirstOrDefaultAsync(pc => pc.Id == photographyCompanyId);
    }

    public async Task<Agent?> GetAgentByEmailAsync(string email)
    {
        return await _context.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == email);
    }

    public async Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        return await _context.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(a => a.AgentFirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Agent>> GetAgentsUnderPhotographyCompanyAsync(string photographyCompanyId)
    {
        return await _context.Agents
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.AgentPhotographyCompanies.Any(apc => apc.PhotographyCompanyId == photographyCompanyId))
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Agents
            .AsNoTracking()
            .CountAsync();
    }

    public async Task<IEnumerable<int>> GetAgentListingCaseIdsAsync(string agentId)
    {
        return await _context.AgentListingCases
            .AsNoTracking()
            .Where(alc => alc.AgentId == agentId)
            .Select(alc => alc.ListingCaseId)
            .ToListAsync();
    }

    public async Task<bool> IsAgentAddedToPhotographyCompanyAsync(string agentId, string photographyCompanyId)
    {
        return await _context.AgentPhotographyCompanies
            .AsNoTracking()
            .AnyAsync(apc => apc.AgentId == agentId && apc.PhotographyCompanyId == photographyCompanyId);
    }

    public async Task<IEnumerable<int>> GetPhotographyCompanyListingCaseIdsAsync(string photographyCompanyId)
    {
        return await _context.ListingCases
            .AsNoTracking()
            .Where(lc => lc.UserId == photographyCompanyId)
            .Select(lc => lc.Id)
            .ToListAsync();
    }
}
