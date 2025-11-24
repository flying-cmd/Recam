using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseRepository : IListingCaseRepository
{
    private readonly AppDbContext _dbContext;

    public ListingCaseRepository(AppDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }

    public async Task<ListingCase> AddListingCaseAsync(ListingCase listingCase)
    {
        await _dbContext.ListingCases.AddAsync(listingCase);
        await _dbContext.SaveChangesAsync();

        return listingCase;
    }

    public async Task<ListingCase?> FindListingCaseByIdAsync(int id)
    {
        return await _dbContext.ListingCases
            .Where(lc => lc.Id == id)
            .Include(lc => lc.MediaAssets)
            .Include(lc => lc.CaseContacts)
            .Include(lc => lc.AgentListingCases)
            .SingleOrDefaultAsync();
    }

    public async Task<User?> FindUserByIdAsync(string userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }
}
