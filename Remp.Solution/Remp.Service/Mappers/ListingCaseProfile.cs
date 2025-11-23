using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class ListingCaseProfile : Profile
{
    public ListingCaseProfile()
    {
        CreateMap<CreateListingCaseRequestDto, ListingCase>();
        CreateMap<ListingCase, CreateListingCaseResponseDto>();
    }
}
