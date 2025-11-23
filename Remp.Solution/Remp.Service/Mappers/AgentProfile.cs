using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class AgentProfile : Profile
{
    public AgentProfile()
    {
        CreateMap<Agent, AgentResponseDto>();
    }
}
