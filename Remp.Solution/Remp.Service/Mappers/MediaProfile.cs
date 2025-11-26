using AutoMapper;
using Remp.Models.Entities;
using Remp.Service.DTOs;

namespace Remp.Service.Mappers;

public class MediaProfile : Profile
{
    public MediaProfile()
    {
        CreateMap<MediaAsset, MediaAssetDto>();
    }
}
