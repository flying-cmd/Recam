using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
    Task<int> CountListingCasesByAgentIdAsync(string currentUserId);
    Task<int> CountListingCasesByPhotographyCompanyIdAsync(string currentUserId);
    Task<ListingCase?> FindListingCaseByListingCaseIdAsync(int id);
    Task<IEnumerable<ListingCase>> FindListingCasesByAgentIdAsync(int pageNumber, int pageSize, string currentUserId);
    Task<IEnumerable<ListingCase>> FindListingCasesByPhotographyCompanyIdAsync(int pageNumber, int pageSize, string currentUserId);
    Task<User?> FindUserByIdAsync(string userId);
}
