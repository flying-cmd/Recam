using Remp.Common.Helpers;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<bool> AddAgentByIdAsync(string agentId, string photographyCompanyId);
    Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId);
}
