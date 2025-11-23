using Microsoft.EntityFrameworkCore;
using Remp.Common.Helpers;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class PhotographyCompanyRepository : IPhotographyCompanyRepository
{
    private readonly AppDbContext _context;

    public PhotographyCompanyRepository(AppDbContext context)
    {
        _context = context;
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
}
