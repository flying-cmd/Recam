using AutoMapper;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IMapper _mapper;

    public AgentService(IAgentRepository agentRepository, IMapper mapper)
    {
        _agentRepository = agentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        var totalCount = await _agentRepository.GetTotalCountAsync();

        var agents = await _agentRepository.GetAgentsAsync(pageNumber, pageSize);

        if (agents == null || !agents.Any())
        {
            throw new NotFoundException(message: $"No agents found. Page number: {pageNumber}, Page size: {pageSize}", title: "No agents found.");
        }

        var agentsDto = _mapper.Map<IEnumerable<AgentResponseDto>>(agents);

        return new PagedResult<AgentResponseDto>(pageNumber, pageSize, totalCount, agentsDto);
    }
}
