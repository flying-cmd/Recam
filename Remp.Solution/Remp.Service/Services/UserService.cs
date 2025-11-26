using AutoMapper;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<bool> AddAgentByIdAsync(string agentId, string photographyCompanyId)
    {
        // Check if the agent exists
        var agent = await _userRepository.FindAgentByIdAsync(agentId);
        if (agent is null)
        {
            throw new NotFoundException(message: $"Agent {agentId} does not exist", title: "Agent does not exist");
        }

        // Check if the photography company already added the agent
        if (await _userRepository.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId))
        {
            return false;
        }

        await _userRepository.AddAgentToPhotographyCompanyAsync(agentId, photographyCompanyId);

        return true;
    }

    public async Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        var totalCount = await _userRepository.GetTotalCountAsync();

        var agents = await _userRepository.GetAgentsAsync(pageNumber, pageSize);

        if (agents == null || !agents.Any())
        {
            throw new NotFoundException(message: $"No agents found. Page number: {pageNumber}, Page size: {pageSize}", title: "No agents found.");
        }

        var agentsDto = _mapper.Map<IEnumerable<AgentResponseDto>>(agents);

        return new PagedResult<AgentResponseDto>(pageNumber, pageSize, totalCount, agentsDto);
    }

    public async Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId)
    {
        return await _userRepository.GetUserListingCaseIdsAsync(currentUserId);
    }
}
