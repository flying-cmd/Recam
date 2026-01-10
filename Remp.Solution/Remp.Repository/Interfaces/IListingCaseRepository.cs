using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IListingCaseRepository
{
    Task AddAgentToListingCaseAsync(AgentListingCase agentListingCase);
    Task<CaseContact> AddCaseContactAsync(CaseContact caseContact);
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
    Task<IEnumerable<MediaAsset>> AddMediaAssetAsync(IEnumerable<MediaAsset> mediaAssets);
    Task<int> CountListingCasesByAgentIdAsync(string currentUserId);
    Task<int> CountListingCasesByPhotographyCompanyIdAsync(string currentUserId);
    Task DeleteAgentFromListingCaseAsync(int listingCaseId, string agentId);
    Task DeleteListingCaseAsync(ListingCase listingCase);
    Task<MediaAsset?> FindCoverImageByListingCaseIdAsync(int listingCaseId);
    Task<ListingCase?> FindListingCaseByListingCaseIdAsync(int id);
    Task<IEnumerable<ListingCase>> FindListingCasesByAgentIdAsync(int pageNumber, int pageSize, string currentUserId);
    Task<IEnumerable<ListingCase>> FindListingCasesByPhotographyCompanyIdAsync(int pageNumber, int pageSize, string currentUserId);
    Task<IEnumerable<MediaAsset>> FindMediaAssetsByListingCaseIdAsync(int listingCaseId);
    Task<MediaAsset?> FindMediaByIdAsync(int mediaAssetId);
    Task<User?> FindUserByIdAsync(string userId);
    Task<IEnumerable<Agent>> GetAssignedAgentByListingCaseIdAsync(int listingCaseId);
    Task<IEnumerable<MediaAsset>> GetFinalSelectionByListingCaseIdAsync(int listingCaseId);
    Task UpdateListingCaseAsync(ListingCase newListingCase);
}
