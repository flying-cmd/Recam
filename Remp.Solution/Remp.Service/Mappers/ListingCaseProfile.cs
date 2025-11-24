using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class ListingCaseProfile : Profile
{
    public ListingCaseProfile()
    {
        CreateMap<CreateListingCaseRequestDto, ListingCase>();
        CreateMap<ListingCase, ListingCaseResponseDto>()
            .ForMember(d => d.PropertyType, opt => opt.MapFrom(src => src.PropertyType.ToString()))
            .ForMember(d => d.SaleCategory, opt => opt.MapFrom(src => src.SaleCategory.ToString()))
            .ForMember(d => d.ListingCaseStatus, opt => opt.MapFrom(src => src.ListingCaseStatus.ToString()));

        CreateMap<ListingCase, ListingCaseDetailResponseDto>()
            .ForMember(d => d.PropertyType, opt => opt.MapFrom(src => src.PropertyType.ToString()))
            .ForMember(d => d.SaleCategory, opt => opt.MapFrom(src => src.SaleCategory.ToString()))
            .ForMember(d => d.ListingCaseStatus, opt => opt.MapFrom(src => src.ListingCaseStatus.ToString()))
            .ForMember(d => d.MediaAssets, opt => opt.MapFrom(src => src.MediaAssets))
            .ForMember(d => d.CaseContacts, opt => opt.MapFrom(src => src.CaseContacts));
    }
}
