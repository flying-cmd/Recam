using AutoMapper;
using Remp.Common.Helpers;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IMapper _mapper;

    public ListingCaseService(IListingCaseRepository listingCaseRepository, IMapper mapper)
    {
        _listingCaseRepository = listingCaseRepository;
        _mapper = mapper;
    }

    public async Task<CreateListingCaseResponseDto> CreateListingCaseAsync(CreateListingCaseRequestDto createListingCaseRequestDto)
    {
        var listingCase = _mapper.Map<ListingCase>(createListingCaseRequestDto);

        listingCase.CreatedAt = DateTime.UtcNow;
        listingCase.IsDeleted = false;
        listingCase.ListingCaseStatus = ListingCaseStatus.Created;

        var createdListingCase = await _listingCaseRepository.AddListingCaseAsync(listingCase);

        // Log
        CaseHistoryLog.LogCreateListingCase(
            listingCaseId: createdListingCase.Id.ToString(),
            userId: createdListingCase.UserId
        );
    
        return _mapper.Map<CreateListingCaseResponseDto>(createdListingCase);
    }
}
