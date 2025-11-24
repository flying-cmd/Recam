using Remp.Service.DTOs;
using System.Security.Claims;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<ListingCaseResponseDto> CreateListingCaseAsync(CreateListingCaseRequestDto createListingCaseRequestDto);
    Task<ListingCaseDetailResponseDto> GetListingCaseByIdAsync(int listingCaseId, string currentUserId, string currrentUserRole);
}
