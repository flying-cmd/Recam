using AutoMapper;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Models.Constants;
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

    public async Task<DeleteListingCaseResponseDto> DeleteListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the owner of the listing case (PhotographyCompany)
        if (listingCase.UserId != currentUserId)
        {
            throw new UnauthorizedException(
                message: $"User {currentUserId} cannot delete this listing case because the user is not the owner of this listing case",
                title: "You cannot delete this listing case because you are not the owner of this listing case"
                );
        }

        try
        {
            await _listingCaseRepository.DeleteListingCaseAsync(listingCase);

            // Log
            CaseHistoryLog.LogDeleteListingCase(
                listingCaseId: listingCase.Id.ToString(),
                userId: currentUserId
            );
        }
        catch (Exception ex)
        {
            var message = $"Listing case {listingCaseId} cannot be deleted because of errors {ex.Message}";

            // Log
            CaseHistoryLog.LogDeleteListingCase(
                listingCaseId: listingCase.Id.ToString(),
                userId: currentUserId,
                description: message
            );
            throw new DeleteException(message: message, title: "Listing case failed to delete");
        }

        return new DeleteListingCaseResponseDto();
    }

    public async Task<PagedResult<ListingCaseResponseDto>> GetAllListingCasesAsync(int pageNumber, int pageSize, string currentUserId, string currrentUserRole)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        IEnumerable<ListingCase> listingCases = [];
        int totalCount = 0;

        // If the user role is PhotographyCompany, return only the listing cases created by the user
        if (currrentUserRole == RoleNames.PhotographyCompany)
        {
            listingCases = await _listingCaseRepository.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, currentUserId);

            if (listingCases == null || !listingCases.Any())
            {
                throw new NotFoundException(
                    message: $"No listing cases created by the photography company {currentUserId} found. Page number: {pageNumber}, Page size: {pageSize}", 
                    title: "No listing cases found."
                    );
            }

            totalCount = await _listingCaseRepository.CountListingCasesByPhotographyCompanyIdAsync(currentUserId);
        }

        // If the user role is Agent, return only the listing cases assigned to the user
        if (currrentUserRole == RoleNames.Agent)
        {
            listingCases = await _listingCaseRepository.FindListingCasesByAgentIdAsync(pageNumber, pageSize, currentUserId);


            if (!listingCases.Any())
            {
                throw new NotFoundException(
                    message: $"No listing cases related to the agent {currentUserId} found. Page number: {pageNumber}, Page size: {pageSize}", 
                    title: "No listing cases found."
                    );
            }

            totalCount = await _listingCaseRepository.CountListingCasesByAgentIdAsync(currentUserId);
        }

        var temp = _mapper.Map<IEnumerable<ListingCaseResponseDto>>(listingCases);
        return new PagedResult<ListingCaseResponseDto>(pageNumber, pageSize, totalCount, _mapper.Map<IEnumerable<ListingCaseResponseDto>>(listingCases));
    }

    public async Task<ListingCaseDetailResponseDto> GetListingCaseByListingCaseIdAsync(int listingCaseId, string userId, string userRole)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
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

    public async Task<IEnumerable<MediaAssetDto>> GetListingCaseMediaByListingCaseIdAsync(int listingCaseId, string userId, string userRole)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
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

        return _mapper.Map<IEnumerable<MediaAssetDto>>(listingCase.MediaAssets);
    }

    public async Task UpdateListingCaseAsync(int listingCaseId, UpdateListingCaseRequestDto updateListingCaseRequest, string userId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the listing case belongs to the user
        if (userId != listingCase.UserId)
        {
            throw new UnauthorizedException(
                message: $"User {userId} cannot update this listing case because the user is not the owner of this listing case",
                title: "You cannot update this listing case because you are not the owner of this listing case"
                );
        }

        // Update the listing case
        // Check if any fields are changed
        var fieldChanges = Checker.CheckChanges(_mapper.Map<UpdateListingCaseRequestDto>(listingCase), updateListingCaseRequest);

        if (fieldChanges.Count == 0)
        {
            // Log
            CaseHistoryLog.LogUpdateListingCase(
                listingCase.Id.ToString(),
                listingCase.UserId.ToString(),
                fieldChanges
                );

            return;
        }

        _mapper.Map(updateListingCaseRequest, listingCase);
        await _listingCaseRepository.UpdateListingCaseAsync(listingCase);

        // Log
        CaseHistoryLog.LogUpdateListingCase(
            listingCase.Id.ToString(),
            listingCase.UserId.ToString(),
            fieldChanges
            );
    }

    // Update the status of the listing case following the workflow
    public async Task UpdateListingCaseStatusAsync(int listingCaseId, string currentUserId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            // Log
            CaseHistoryLog.LogUpdateListingCaseStatus(
                listingCaseId.ToString(),
                currentUserId,
                null,
                null,
                $"User {currentUserId} failed to update the status of listing case {listingCaseId} because the it does not exist"
                );

            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Update the listing case status
        var oldStatus = listingCase.ListingCaseStatus;

        // If the oldStatus is Delivered, it will not be updated
        if (oldStatus == ListingCaseStatus.Delivered)
        {
            // Log
            CaseHistoryLog.LogUpdateListingCaseStatus(
                listingCaseId.ToString(),
                currentUserId,
                oldStatus.ToString(),
                oldStatus.ToString(),
                $"User {currentUserId} failed to update the status of listing case {listingCaseId} because the it is already delivered"
                );

            throw new ArgumentErrorException(message: $"Listing case {listingCaseId} cannot be updated because it is already delivered", title: "Listing case cannot be updated because it is already delivered");
        }

        var newStatus = (ListingCaseStatus)((int)listingCase.ListingCaseStatus + 1);
        listingCase.ListingCaseStatus = newStatus;
        await _listingCaseRepository.UpdateListingCaseStatusAsync(listingCase);

        // Log
        CaseHistoryLog.LogUpdateListingCaseStatus(
            listingCase.Id.ToString(),
            currentUserId,
            oldStatus.ToString(),
            newStatus.ToString()
            );
    }
}
