using AutoMapper;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Security.Claims;

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

    public async Task<ListingCaseResponseDto> CreateListingCaseAsync(CreateListingCaseRequestDto createListingCaseRequestDto)
    {
        // Check if the user exists
        var user = await _listingCaseRepository.FindUserByIdAsync(createListingCaseRequestDto.UserId);
        if (user is null)
        {
            throw new NotFoundException(message: "User does not exist", title: "User does not exist");
        }

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
    
        return _mapper.Map<ListingCaseResponseDto>(createdListingCase);
    }

    public async Task<ListingCaseDetailResponseDto> GetListingCaseByIdAsync(int listingCaseId, string userId, string userRole)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the owner of the listing case (PhotographyCompany)
        if (userRole == RoleNames.PhotographyCompany && listingCase.UserId != userId)
        {
            throw new UnauthorizedException(
                message: $"User {userId} cannot access this listing case because the user is not the owner of this listing case",
                title: "You cannot access this listing case because you are not the owner of this listing case"
                );
        }

        // Check if the user is the assigned agent
        if (userRole == RoleNames.Agent && listingCase.AgentListingCases.Any(x => x.AgentId != userId))
        {
            throw new UnauthorizedException(
                message: $"User {userId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        return _mapper.Map<ListingCaseDetailResponseDto>(listingCase);
    }
}
