using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class AgentProfile : Profile
{
    public AgentProfile()
    {
        CreateMap<Agent, CreateAgentAccountResponseDto>();

        CreateMap<Agent, SearchAgentResponseDto>()
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(d => d.Email, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<Agent, AgentDto>();
    }
}
