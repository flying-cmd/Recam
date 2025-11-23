using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
}
