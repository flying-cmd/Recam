using Remp.Common.Helpers;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAgentService
{
    Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize);
}
