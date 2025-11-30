using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class ListingCaseRepository : IListingCaseRepository
{
    private readonly AppDbContext _dbContext;

    public ListingCaseRepository(AppDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }

    public async Task<CaseContact> AddCaseContactAsync(CaseContact caseContact)
    {
        await _dbContext.CaseContacts.AddAsync(caseContact);
        await _dbContext.SaveChangesAsync();

        return caseContact;
    }

    public async Task<ListingCase> AddListingCaseAsync(ListingCase listingCase)
    {
        await _dbContext.ListingCases.AddAsync(listingCase);
        await _dbContext.SaveChangesAsync();

        return listingCase;
    }

    public async Task<IEnumerable<MediaAsset>> AddMediaAssetAsync(IEnumerable<MediaAsset> mediaAssets)
    {
        await _dbContext.MediaAssets.AddRangeAsync(mediaAssets);
        await _dbContext.SaveChangesAsync();

        return mediaAssets;
    }

    public Task<int> CountListingCasesByAgentIdAsync(string currentUserId)
    {
        return _dbContext.ListingCases
            .AsNoTracking()
            .Where(lc => lc.AgentListingCases.Any(alc => alc.AgentId == currentUserId))
            .CountAsync();
    }

    public Task<int> CountListingCasesByPhotographyCompanyIdAsync(string currentUserId)
    {
        return _dbContext.ListingCases
            .AsNoTracking()
            .Where(lc => lc.UserId == currentUserId)
            .CountAsync();
    }

    public async Task DeleteListingCaseAsync(ListingCase listingCase)
    {
        _dbContext.ListingCases.Remove(listingCase);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ListingCase?> FindListingCaseByListingCaseIdAsync(int id)
    {
        return await _dbContext.ListingCases
            .Where(lc => lc.Id == id)
            .Include(lc => lc.MediaAssets)
            .Include(lc => lc.CaseContacts)
            .Include(lc => lc.AgentListingCases)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<ListingCase>> FindListingCasesByAgentIdAsync(int pageNumber, int pageSize, string currentUserId)
    {
        return await _dbContext.ListingCases
            .Where(lc => lc.AgentListingCases.Any(alc => alc.AgentId == currentUserId))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<ListingCase>> FindListingCasesByPhotographyCompanyIdAsync(int pageNumber, int pageSize, string currentUserId)
    {
        return await _dbContext.ListingCases
            .Where(lc => lc.UserId == currentUserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> FindUserByIdAsync(string userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    public async Task UpdateListingCaseAsync(ListingCase newListingCase)
    {
        _dbContext.ListingCases.Update(newListingCase);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateListingCaseStatusAsync(ListingCase newListingCase)
    {
        await UpdateListingCaseAsync(newListingCase);
    }
}
