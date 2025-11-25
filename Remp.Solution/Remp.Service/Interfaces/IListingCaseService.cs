using Remp.Common.Helpers;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<ListingCaseResponseDto> CreateListingCaseAsync(CreateListingCaseRequestDto createListingCaseRequestDto);
    Task<DeleteListingCaseResponseDto> DeleteListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId);
    Task<PagedResult<ListingCaseResponseDto>> GetAllListingCasesAsync(int pageNumer, int pageSize, string currentUserId, string currrentUserRole);
    Task<ListingCaseDetailResponseDto> GetListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId, string currrentUserRole);
    Task UpdateListingCaseAsync(int listingCaseId, UpdateListingCaseRequestDto updateListingCaseRequest, string userId);
}
