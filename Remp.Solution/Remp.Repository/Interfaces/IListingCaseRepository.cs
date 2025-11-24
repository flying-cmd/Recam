using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
    Task<User?> FindUserByIdAsync(string userId);
}
