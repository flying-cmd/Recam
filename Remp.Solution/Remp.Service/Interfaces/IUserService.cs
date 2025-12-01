using Remp.Common.Helpers;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<bool> AddAgentByIdAsync(string agentId, string photographyCompanyId);
    Task<CreateAgentAccountResponseDto?> CreateAgentAccountAsync(CreateAgentAccountRequestDto createAgentAccountRequestDto, string photographyCompanyId);
    Task<SearchAgentResponseDto?> GetAgentByEmailAsync(string email);
    Task<PagedResult<CreateAgentAccountResponseDto>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<SearchAgentResponseDto>> GetAgentsUnderPhotographyCompanyAsync(string photographyCompanyId);
    Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId);
    Task<UpdateApiResponse> UpdatePasswordAsync(UpdatePasswordRequestDto updatePasswordRequestDto, string userId);
}
