using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class CaseContactProfile : Profile
{
    public CaseContactProfile()
    {
        CreateMap<CaseContact, CaseContactDto>();
    }
}
