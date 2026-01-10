using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Common.Utilities;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IO.Compression;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly IListingCaseRepository _listingCaseRepository;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _loggerService;

    public ListingCaseService(
        IListingCaseRepository listingCaseRepository, 
        IMapper mapper, 
        IBlobStorageService blobStorageService,
        IMediaRepository mediaRepository,
        IUserRepository userRepository,
        IConfiguration configuration,
        ILoggerService loggerService)
    {
        _listingCaseRepository = listingCaseRepository;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
        _mediaRepository = mediaRepository;
        _userRepository = userRepository;
        _configuration = configuration;
        _loggerService = loggerService;
    }

    public async Task<CaseContactDto> CreateCaseContactByListingCaseIdAsync(int listingCaseId, CreateCaseContactRequestDto createCaseContactRequest)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        var caseContact = _mapper.Map<CaseContact>(createCaseContactRequest);
        caseContact.ListingCaseId = listingCaseId;

        var createdCaseContact = await _listingCaseRepository.AddCaseContactAsync(caseContact);

        return _mapper.Map<CaseContactDto>(createdCaseContact);
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
        await _loggerService.LogCreateListingCase(
            listingCaseId: createdListingCase.Id.ToString(),
            userId: createdListingCase.UserId
        );
    
        return _mapper.Map<ListingCaseResponseDto>(createdListingCase);
    }

    public async Task<IEnumerable<MediaAssetDto>> CreateMediaByListingCaseIdAsync(IEnumerable<IFormFile> files, MediaType mediaType, int listingCaseId, string userId)
    {
        // Check the quantity of files
        // Only pictures allows multiple
        if (mediaType != MediaType.Photo && files.Count() > 1)
        {
            throw new ArgumentErrorException(message: "Only picture type allow multiple file upload", title: "Only picture type allow multiple file upload");
        }

        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the owner of the listing case (PhotographyCompany)
        if (listingCase.UserId != userId)
        {
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the owner of this listing case",
                title: "You cannot access this listing case because you are not the owner of this listing case"
                );
        }

        var createdMediaAssets = new List<MediaAsset>();
        var uploadedBlobUrls = new List<string>();

        try
        {
            // Upload files to the blob storage
            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                var blobUrl = await _blobStorageService.UploadFileAsync(file);

                uploadedBlobUrls.Add(blobUrl);

                // Create media asset and save it to the database
                var mediaAssets = new MediaAsset
                {
                    MediaType = mediaType,
                    MediaUrl = blobUrl,
                    UploadedAt = DateTime.UtcNow,
                    IsSelect = false,
                    IsHero = false,
                    ListingCaseId = listingCaseId,
                    UserId = userId
                };

                createdMediaAssets.Add(mediaAssets);
            }

            // Save media asset to the database
            var result = await _listingCaseRepository.AddMediaAssetAsync(createdMediaAssets);

            return _mapper.Map<IEnumerable<MediaAssetDto>>(result);
        }
        catch
        {
            // If not all files are uploaded, delete the uploaded files from the blob storage
            // If uploads successfully but save to the database failed, delete the uploaded files from the blob storage
            foreach (var url in uploadedBlobUrls)
            {
                await _blobStorageService.DeleteFileAsync(url);
            }

            throw;
        }
        
    }

    public async Task DeleteListingCaseByListingCaseIdAsync(int listingCaseId, string currentUserId)
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
            throw new ForbiddenException(
                message: $"User {currentUserId} cannot delete this listing case because the user is not the owner of this listing case",
                title: "You cannot delete this listing case because you are not the owner of this listing case"
                );
        }

        // Delete all files from the blob storage
        foreach (var mediaAsset in listingCase.MediaAssets)
        {
            await _blobStorageService.DeleteFileAsync(mediaAsset.MediaUrl);
        }

        await _listingCaseRepository.DeleteListingCaseAsync(listingCase);

        // Log
        await _loggerService.LogDeleteListingCase(
            listingCaseId: listingCase.Id.ToString(),
            userId: currentUserId
        );
    }

    public async Task<(byte[] ZipContent, string ContentType, string ZipFileName)> DownloadAllMediaByListingCaseIdAsync(int listingCaseId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Get all media assets by listing case id
        var mediaAssets = await _listingCaseRepository.FindMediaAssetsByListingCaseIdAsync(listingCaseId);
    
        if (!mediaAssets.Any())
        {
            throw new NotFoundException(message: $"No media assets found for listing case {listingCaseId}", title: "No media assets found");
        }

        // ZIP the media assets
        var zipStream = new MemoryStream();

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var media in mediaAssets)
            {
                // Download the media file from Azure Blob
                var (content, contentType, fileName) = await _blobStorageService.DownloadFileAsync(media.MediaUrl);

                // Create a new entry in the zip archive
                var zipEntry = zip.CreateEntry(fileName);

                // Copy the media file content to the zip entry
                using var zipStreamEntry = zipEntry.Open();

                await content.CopyToAsync(zipStreamEntry);
            }
        }

        // Reset the position to the beginning of the stream
        zipStream.Position = 0;

        var zipContentType = "application/zip";

        return (zipStream.ToArray(), zipContentType, $"listingcase_{listingCaseId}_media.zip");
    }

    public async Task<string> GenerateSharedUrlAsync(int listingCaseId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        var baseUrl = _configuration["Url:BaseUrl"];
        var randomUrl = Guid.NewGuid().ToString("N"); // "N" format removes hyphens
        var sharedUrl = $"{baseUrl}/listings/share/{randomUrl}";

        listingCase.SharedUrl = sharedUrl;

        await _listingCaseRepository.UpdateListingCaseAsync(listingCase);
    
        return sharedUrl;
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

            if (!listingCases.Any())
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

        return new PagedResult<ListingCaseResponseDto>(pageNumber, pageSize, totalCount, _mapper.Map<IEnumerable<ListingCaseResponseDto>>(listingCases));
    }

    public async Task<IEnumerable<MediaAssetDto>> GetFinalSelectionByListingCaseIdAsync(int listingCaseId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check the status of the listing case
        if (listingCase.ListingCaseStatus != ListingCaseStatus.Delivered)
        {
            throw new InvalidException(
                message: $"Listing case {listingCaseId} is not delivered, so you cannot get the final selection", 
                title: "Listing case is not delivered, so you cannot get the final selection");
        }

        var mediaAsset = await _listingCaseRepository.GetFinalSelectionByListingCaseIdAsync(listingCaseId);

        return _mapper.Map<IEnumerable<MediaAssetDto>>(mediaAsset);
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
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the owner of this listing case",
                title: "You cannot access this listing case because you are not the owner of this listing case"
                );
        }

        // Check if the user is the assigned agent
        if (userRole == RoleNames.Agent && listingCase.AgentListingCases.Any(x => x.AgentId != userId))
        {
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        return _mapper.Map<ListingCaseDetailResponseDto>(listingCase);
    }

    public async Task<IEnumerable<CaseContactDto>> GetListingCaseContactByListingCaseIdAsync(int listingCaseId, string userId, string userRole)
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
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the owner of this listing case",
                title: "You cannot access this listing case because you are not the owner of this listing case"
                );
        }

        // Check if the user is the assigned agent
        if (userRole == RoleNames.Agent && listingCase.AgentListingCases.Any(x => x.AgentId != userId))
        {
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        return _mapper.Map<IEnumerable<CaseContactDto>>(listingCase.CaseContacts);
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
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the owner of this listing case",
                title: "You cannot access this listing case because you are not the owner of this listing case"
                );
        }

        // Check if the user is the assigned agent
        if (userRole == RoleNames.Agent && listingCase.AgentListingCases.Any(x => x.AgentId != userId))
        {
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        return _mapper.Map<IEnumerable<MediaAssetDto>>(listingCase.MediaAssets);
    }

    public async Task SetCoverImageByListingCaseIdAsync(int listingCaseId, int mediaAssetId, string userId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the assigned agent
        if (!listingCase.AgentListingCases.Any(x => x.AgentId == userId))
        {
            throw new ForbiddenException(
                message: $"User {mediaAssetId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        // Check if the media asset exists
        var mediaAsset = await _listingCaseRepository.FindMediaByIdAsync(mediaAssetId);
        if (mediaAsset is null)
        {
            throw new NotFoundException(message: $"Media asset {mediaAssetId} does not exist", title: "Media asset does not exist");
        }

        // Check if the media asset is a picture
        if (mediaAsset.MediaType != MediaType.Photo)
        {
            throw new ArgumentErrorException(message: $"Media asset {mediaAssetId} is not a picture", title: "Media asset is not a picture");
        }

        // Check if the media asset belongs to the listing case
        if (mediaAsset.ListingCaseId != listingCaseId)
        {
            throw new ArgumentErrorException(message: $"Media asset {mediaAssetId} does not belong to this listing case", title: "Media asset does not belong to this listing case");
        }

        var updatedMediaAssets = new List<MediaAsset> { mediaAsset };

        // Check if the listing case already has a cover image
        var existingCoverImage = await _listingCaseRepository.FindCoverImageByListingCaseIdAsync(listingCaseId);
        if (existingCoverImage is not null)
        {
            // Change the existing cover image to be not the cover image
            existingCoverImage.IsHero = false;

            updatedMediaAssets.Add(existingCoverImage);
        }

        // Set the selected media asset as the cover image
        mediaAsset.IsHero = true;
        mediaAsset.IsSelect = true;
        await _mediaRepository.UpdateMediaAssetsAsync(updatedMediaAssets);
    }

    public async Task SetSelectedMediaByListingCaseIdAsync(int listingCaseId, IEnumerable<int> mediaIds, string userId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the assigned agent
        if (!listingCase.AgentListingCases.Any(x => x.AgentId == userId))
        {
            throw new ForbiddenException(
                message: $"User {userId} cannot access this listing case because the user is not the assigned agent of this listing case",
                title: "You cannot access this listing case because you are not the assigned agent of this listing case"
                );
        }

        // If selected media assets contains photots, the maximum number of photos is 10
        var mediaAssets = await _mediaRepository.FindMediaByIdsAsync(mediaIds);
        if (mediaAssets.Count(x => x.MediaType == MediaType.Photo) > 10)
        {
            throw new ArgumentErrorException(message: $"You can only select up to 10 photos", title: "You can only select up to 10 photos");
        }

        // Check if all media assets belong to the listing case
        if (mediaAssets.Any(x => x.ListingCaseId != listingCaseId))
        {
            throw new ArgumentErrorException(message: $"All media assets must belong to this listing case", title: "All media assets must belong to this listing case");
        }

        // Set the media assets as selected and other media assets as not selected
        var idSet = new HashSet<int>(mediaIds);
        foreach (var media in listingCase.MediaAssets)
        {
            media.IsSelect = idSet.Contains(media.Id);
        }

        await _mediaRepository.UpdateMediaAssetsAsync(listingCase.MediaAssets);

        // Log
        await _loggerService.LogSelectMedia(
            listingCaseId.ToString(),
            mediaIds,
            userId
            );
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
            throw new ForbiddenException(
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
            await _loggerService.LogUpdateListingCase(
                listingCase.Id.ToString(),
                listingCase.UserId.ToString(),
                fieldChanges
                );

            return;
        }

        _mapper.Map(updateListingCaseRequest, listingCase);
        await _listingCaseRepository.UpdateListingCaseAsync(listingCase);

        // Log
        await _loggerService.LogUpdateListingCase(
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
            await _loggerService.LogUpdateListingCaseStatus(
                listingCaseId.ToString(),
                currentUserId,
                null,
                null,
                error: $"Listing case {listingCaseId} does not exist"
                );

            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the owner of the listing case (PhotographyCompany)
        if (listingCase.UserId != currentUserId)
        {
            // Log
            await _loggerService.LogUpdateListingCaseStatus(
                listingCaseId.ToString(),
                currentUserId,
                null,
                null,
                error: $"User is not the owner of this listing case"
                );

            throw new ForbiddenException(
                message: $"User {currentUserId} cannot update this listing case because the user is not the owner of this listing case",
                title: "You cannot update this listing case because you are not the owner of this listing case"
                );
        }

        // Update the listing case status
        var oldStatus = listingCase.ListingCaseStatus;

        // If the oldStatus is Delivered, it will not be updated
        if (oldStatus == ListingCaseStatus.Delivered)
        {
            // Log
            await _loggerService.LogUpdateListingCaseStatus(
                listingCaseId: listingCaseId.ToString(),
                userId: currentUserId,
                oldStatus: oldStatus.ToString(),
                newStatus: oldStatus.ToString(),
                error: $"Status is already Delivered"
                );

            throw new ArgumentErrorException(message: $"Listing case {listingCaseId} cannot be updated because it is already delivered", title: "Listing case cannot be updated because it is already delivered");
        }

        var newStatus = listingCase.ListingCaseStatus.NextStatus();
        listingCase.ListingCaseStatus = newStatus;
        await _listingCaseRepository.UpdateListingCaseAsync(listingCase);

        // Log
        await _loggerService.LogUpdateListingCaseStatus(
            listingCase.Id.ToString(),
            currentUserId,
            oldStatus.ToString(),
            newStatus.ToString()
            );
    }

    public async Task AddAgentToListingCaseAsync(int listingCaseId, string agentId, string userId)
    {
        // Check if the current user exists
        var photographyCompany = await _userRepository.FindPhotographyCompanyByIdAsync(userId);
        if (photographyCompany is null)
        {
            // Log
            await _loggerService.LogAddAgentToListingCase(
                listingCaseId: listingCaseId.ToString(),
                agentId: agentId,
                photographyCompanyId: null,
                error: $"Photography company {userId} does not exist"
                );

            throw new UnauthorizedException(message: $"Photography company {userId} does not exist", title: "Photography company does not exist");
        }

        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);
        if (listingCase is null)
        {
            // Log
            await _loggerService.LogAddAgentToListingCase(
                listingCaseId: null,
                agentId: agentId,
                photographyCompanyId: userId,
                error: $"Listing case {listingCaseId} does not exist"
                );

            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        // Check if the user is the owner of the listing case (PhotographyCompany)
        if (listingCase.UserId != userId)
        {
            // Log
            await _loggerService.LogAddAgentToListingCase(
                listingCaseId: listingCaseId.ToString(),
                agentId: agentId,
                photographyCompanyId: userId,
                error: $"User is not the owner of this listing case"
                );

            throw new ForbiddenException(
                message: $"User {userId} cannot add agent to this listing case because the user is not the owner of this listing case",
                title: "You cannot add agent to this listing case because you are not the owner of this listing case"
                );
        }

        // Check if the agent exists
        var agent = await _userRepository.FindAgentByIdAsync(agentId);
        if (agent is null)
        {
            // Log
            await _loggerService.LogAddAgentToListingCase(
                listingCaseId: listingCaseId.ToString(),
                agentId: null,
                photographyCompanyId: userId,
                error: $"Agent {agentId} does not exist"
                );

            throw new NotFoundException(message: $"Agent {agentId} does not exist", title: "Agent does not exist");
        }

        // Add agent to listing case
        AgentListingCase agentListingCase = new AgentListingCase() { AgentId = agentId, ListingCaseId = listingCaseId };
        await _listingCaseRepository.AddAgentToListingCaseAsync(agentListingCase);
    }

    public async Task<IEnumerable<SearchAgentResponseDto>> GetAssignedAgentByListingCaseIdAsync(int listingCaseId)
    {
        // Check if the listing case exists
        var listingCase = await _listingCaseRepository.FindListingCaseByListingCaseIdAsync(listingCaseId);

        if (listingCase is null)
        {
            throw new NotFoundException(message: $"Listing case {listingCaseId} does not exist", title: "Listing case does not exist");
        }

        var assignedAgent = await _listingCaseRepository.GetAssignedAgentByListingCaseIdAsync(listingCaseId);

        return _mapper.Map<IEnumerable<SearchAgentResponseDto>>(assignedAgent);
    }
}
