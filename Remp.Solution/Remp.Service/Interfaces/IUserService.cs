using Remp.Common.Helpers;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task AddAgentByIdAsync(string agentId, string photographyCompanyId);
    Task<CreateAgentAccountResponseDto?> CreateAgentAccountAsync(CreateAgentAccountRequestDto createAgentAccountRequestDto, string photographyCompanyId);
    Task DeleteAssignedAgentByAgentIdAsync(string agentId, string currentUserId);
    Task<SearchAgentResponseDto?> GetAgentByEmailAsync(string email);
    Task<PagedResult<SearchAgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<SearchAgentResponseDto>> GetAgentsUnderPhotographyCompanyAsync(string photographyCompanyId);
    Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId, string userRole);
    Task UpdatePasswordAsync(UpdatePasswordRequestDto updatePasswordRequestDto, string userId);
}
