using AutoMapper;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class PhotographyCompanyService : IPhotographyCompanyService
{
    private readonly IPhotographyCompanyRepository _photographyCompanyRepository;
    private readonly IMapper _mapper;

    public PhotographyCompanyService(IPhotographyCompanyRepository photographyCompanyRepository, IMapper mapper)
    {
        _photographyCompanyRepository = photographyCompanyRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        var totalCount = await _photographyCompanyRepository.GetTotalCountAsync();

        var agents = await _photographyCompanyRepository.GetAgentsAsync(pageNumber, pageSize);

        if (agents == null || !agents.Any())
        {
            throw new NotFoundException(message: $"No agents found. Page number: {pageNumber}, Page size: {pageSize}", title: "No agents found.");
        }

        var agentsDto = _mapper.Map<IEnumerable<AgentResponseDto>>(agents);

        return new PagedResult<AgentResponseDto>(pageNumber, pageSize, totalCount, agentsDto);
    }
}
