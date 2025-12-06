using Microsoft.AspNetCore.Http;
using Remp.Common.Helpers;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<CaseContactDto> CreateCaseContactByListingCaseIdAsync(int listingCaseId, CreateCaseContactRequestDto createCaseContactRequest);
    Task<ListingCaseResponseDto> CreateListingCaseAsync(CreateListingCaseRequestDto createListingCaseRequestDto);
    Task<IEnumerable<MediaAssetDto>> CreateMediaByListingCaseIdAsync(IEnumerable<IFormFile> files, MediaType mediaType, int listingCaseId, string userId);
    Task DeleteListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId);
    Task<(byte[] ZipContent, string ContentType, string ZipFileName)> DownloadAllMediaByListingCaseIdAsync(int listingCaseId);
    Task<string> GenerateSharedUrlAsync(int listingCaseId);
    Task<PagedResult<ListingCaseResponseDto>> GetAllListingCasesAsync(int pageNumer, int pageSize, string currentUserId, string currrentUserRole);
    Task<IEnumerable<MediaAssetDto>> GetFinalSelectionByListingCaseIdAsync(int listingCaseId);
    Task<ListingCaseDetailResponseDto> GetListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId, string currrentUserRole);
    Task<IEnumerable<CaseContactDto>> GetListingCaseContactByListingCaseIdAsync(int listingCaseId, string userId, string userRole);
    Task<IEnumerable<MediaAssetDto>> GetListingCaseMediaByListingCaseIdAsync(int listingCaseId, string userId, string userRole);
    Task SetCoverImageByListingCaseIdAsync(int listingCaseId, int mediaAssetId, string userId);
    Task SetSelectedMediaByListingCaseIdAsync(int listingCaseId, IEnumerable<int> mediaIds, string userId);
    Task UpdateListingCaseAsync(int listingCaseId, UpdateListingCaseRequestDto updateListingCaseRequest, string userId);
    Task UpdateListingCaseStatusAsync(int listingCaseId, string currentUserId);
}
