using Remp.Common.Helpers;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize);
}
