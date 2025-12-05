using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
using Remp.Common.Helpers.ApiResponse;
using Remp.Common.Utilities;
using Remp.Models.Constants;
using Remp.Models.Enums;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Security.Claims;

namespace Remp.API.Controllers
{
    [Route("api/listings")]
    [ApiController]
    public class ListingCaseController : ControllerBase
    {
        private readonly IListingCaseService _listingCaseService;
        private readonly ILoggerService _loggerService;

        public ListingCaseController(IListingCaseService listingCaseService, ILoggerService loggerService)
        {
            _listingCaseService = listingCaseService;
            _loggerService = loggerService;
        }

        /// <summary>
        /// Get a listing case by id
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case to retrieve.
        /// </param>
        /// <returns>
        /// Returns the retrieved listing case.
        /// </returns>
        /// <response code="200">Returns <see cref="ListingCaseResponseDto"/></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">The listing case was not found</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> or <c>Agent</c> roles.
        /// </remarks>
        [HttpGet("{listingCaseId:int}", Name = "GetListingCaseByListingCaseIdAsync")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<ListingCaseDetailResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetResponse<ListingCaseDetailResponseDto>>> GetListingCaseByListingCaseIdAsync(int listingCaseId)
        {
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }
            
            var currrentUserRole = currentUser.FindFirstValue("scopes");
            if (currrentUserRole == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.GetListingCaseByListingCaseIdAsync(listingCaseId, currentUserId, currrentUserRole);
            
            return Ok(new GetResponse<ListingCaseDetailResponseDto>(true, result));
        }

        /// <summary>
        /// Create a new listing case
        /// </summary>
        /// <param name="createListingCaseRequest">
        /// The payload containing the details of the listing case to create.
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the created listing case and a location header pointing to access it.
        /// </returns>
        /// <response code="201">Listing case created.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Request validation failed.</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostResponse<ListingCaseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostResponse<ListingCaseResponseDto>>> CreateListingCase(
            [FromBody] CreateListingCaseRequestDto createListingCaseRequest, 
            IValidator<CreateListingCaseRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createListingCaseRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                // Log
                await _loggerService.LogCreateListingCase(
                    listingCaseId: null,
                    userId: createListingCaseRequest.UserId,
                    error: errors
                );

                return ValidationProblem(ModelState);
            }

            var result = await _listingCaseService.CreateListingCaseAsync(createListingCaseRequest);
            
            return CreatedAtRoute(
                nameof(GetListingCaseByListingCaseIdAsync), 
                new { listingCaseId = result.Id }, 
                new PostResponse<ListingCaseResponseDto>(true, result)
                );
        }

        /// <summary>
        /// Get all listing cases in pagination.
        /// If the user is an agent, it will return the listing cases that are assigned to the agent.
        /// If the user is a photography company, it will return the listing cases that are created by the photography company.
        /// </summary>
        /// <param name="pageNumer">Page number</param>
        /// <param name="pageSize">Page size (how many items per page)</param>
        /// <returns>
        /// Returns a list of listing cases, current page number, page size, total pages and total count
        /// </returns>
        /// <response code="200">Returns a list of listing cases, current page number, page size, total pages and total count</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Invalid page number or page size</response>
        /// <response code="404">No listing cases found</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> or <c>Agent</c> roles.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<PagedResult<ListingCaseResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetResponse<PagedResult<ListingCaseResponseDto>>>> GetAllListingCasesAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
        {
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var currrentUserRole = currentUser.FindFirstValue("scopes");
            if (currrentUserRole == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.GetAllListingCasesAsync(pageNumer, pageSize, currentUserId, currrentUserRole);

            return Ok(new GetResponse<PagedResult<ListingCaseResponseDto>>(true, result));
        }

        /// <summary>
        /// Update a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case to update.
        /// </param>
        /// <param name="updateListingCaseRequest">
        /// The payload containing the details of the listing case to update.
        /// </param>
        /// <param name="validator"></param>
        /// <returns></returns>
        /// <response code="204">Listing case updated.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Request validation failed.</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpPut("{listingCaseId}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(PutResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PutResponse>> UpdateListingCaseAsync(int listingCaseId, [FromBody] UpdateListingCaseRequestDto updateListingCaseRequest, IValidator<UpdateListingCaseRequestDto> validator)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            // Validate
            var validationResult = await validator.ValidateAsync(updateListingCaseRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                // Log
                await _loggerService.LogUpdateListingCase(
                    listingCaseId: listingCaseId.ToString(),
                    userId: currentUserId,
                    updatedFields: null,
                    error: errors
                );

                return ValidationProblem(ModelState);
            }

            await _listingCaseService.UpdateListingCaseAsync(listingCaseId, updateListingCaseRequest, currentUserId);
            
            return StatusCode(204, new PutResponse(true));
        }

        /// <summary>
        /// Delete a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case to delete
        /// </param>
        /// <returns>If success, returns a message "Deleted successfully."</returns>
        /// <response code="204">Listing case deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to delete listing case</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpDelete("{listingCaseId:int}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DeleteResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<DeleteResponse>> DeleteListingCaseByListingCaseIdAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            await _listingCaseService.DeleteListingCaseByListingCaseIdAsync(listingCaseId, currentUserId);

            return StatusCode(204, new DeleteResponse(true));
        }

        /// <summary>
        /// Update listing case status
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>If success, returns a message "Updated successfully."</returns>
        /// <response code="204">Listing case status updated</response>
        /// <response code="400">Failed to update listing case status</response>
        /// <remarks>
        /// Updating listing case status must follow the workflow of the listing case (Created -> Pending -> Delivered).
        /// </remarks>
        [HttpPatch("{listingCaseId:int}/status")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(PutResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PutResponse>> UpdateListingCaseStatusAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            await _listingCaseService.UpdateListingCaseStatusAsync(listingCaseId, currentUserId);

            return StatusCode(204, new PutResponse(true));
        }

        /// <summary>
        /// Get all media assets of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>
        /// Returns a list of media assets
        /// </returns>
        /// <response code="200">Returns a list of media assets</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to get media assets</response>
        /// <response code="404">Media assets not found</response>
        /// <remarks>
        /// This endpoint is restricted to the phtography companiey who created the listing case and the agent who is assigned the listing case.
        /// </remarks>
        [HttpGet("{listingCaseId:int}/media", Name = "GetListingCaseMediaByListingCaseIdAsync")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<IEnumerable<MediaAssetDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetResponse<IEnumerable<MediaAssetDto>>>> GetListingCaseMediaByListingCaseIdAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            // Get current user role
            var currentUserRole = currentUser.FindFirstValue("scopes");
            if (currentUserRole == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.GetListingCaseMediaByListingCaseIdAsync(listingCaseId, currentUserId, currentUserRole);

            return Ok(new GetResponse<IEnumerable<MediaAssetDto>>(true, result));
        }

        /// <summary>
        /// Get all contact information of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>
        /// Returns a list of contact information
        /// </returns>
        /// <response code="200">Returns a list of contact information</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to get contact information</response>
        /// <response code="404">Contact not found</response>
        /// <remarks>
        /// This endpoint is restricted to the phtography companiey who created the listing case and the agent who is assigned the listing case.
        /// </remarks>
        [HttpGet("{listingCaseId:int}/contact", Name = "GetListingCaseContactByListingCaseIdAsync")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<IEnumerable<CaseContactDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetResponse<IEnumerable<CaseContactDto>>>> GetListingCaseContactByListingCaseIdAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            // Get current user role
            var currentUserRole = currentUser.FindFirstValue("scopes");
            if (currentUserRole == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.GetListingCaseContactByListingCaseIdAsync(listingCaseId, currentUserId, currentUserRole);

            return Ok(new GetResponse<IEnumerable<CaseContactDto>>(true, result));
        }

        /// <summary>
        /// Create contact information of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <param name="createCaseContactRequest">
        /// The payload containing the contact information
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the created contact information
        /// </returns>
        /// <response code="201">Returns the created contact information</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to create contact information</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>Agent</c> role.
        /// </remarks>
        [HttpPost("{listingCaseId:int}/contact")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostResponse<CaseContactDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<CaseContactDto>>> CreateCaseContactByListingCaseIdAsync(
            int listingCaseId, 
            [FromBody] CreateCaseContactRequestDto createCaseContactRequest,
            IValidator<CreateCaseContactRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createCaseContactRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                
                return ValidationProblem(ModelState);
            }

            var result = await _listingCaseService.CreateCaseContactByListingCaseIdAsync(listingCaseId, createCaseContactRequest);
            
            return CreatedAtRoute(
                nameof(GetListingCaseContactByListingCaseIdAsync), 
                new { listingCaseId }, 
                new PostResponse<CaseContactDto>(true, result));
        }

        /// <summary>
        /// Upload media to a listing case
        /// </summary>
        /// <param name="createMediaRequestDto">
        /// The payload containing the media and the media type
        /// </param>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the created media
        /// </returns>
        /// <response code="201">Returns the created media</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to create media</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpPost("{listingCaseId:int}/media")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostResponse<IEnumerable<MediaAssetDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<IEnumerable<MediaAssetDto>>>> CreateMediaByListingCaseIdAsync(
            [FromForm] CreateMediaRequestDto createMediaRequestDto,
            int listingCaseId,
            IValidator<CreateMediaRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createMediaRequestDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                return ValidationProblem(ModelState);
            }

            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.CreateMediaByListingCaseIdAsync(createMediaRequestDto.MediaFiles, (MediaType)Enum.Parse(typeof(MediaType), createMediaRequestDto.MediaType), listingCaseId, currentUserId);
            
            return CreatedAtRoute(
                nameof(GetListingCaseMediaByListingCaseIdAsync), 
                new { listingCaseId }, 
                new PostResponse<IEnumerable<MediaAssetDto>>(true, result));
        }

        /// <summary>
        /// Download all media assets of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>
        /// Returns the zip file containing all media assets
        /// </returns>
        /// <response code="200">Returns the zip file containing all media assets</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to download media assets</response>
        [HttpGet("{listingCaseId:int}/download")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [Produces("application/zip")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DownloadAllMediaByListingCaseIdAsync(int listingCaseId)
        {
            var (zipStream, contentType, fileName) = await _listingCaseService.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

            return File(zipStream, contentType, fileName);
        }

        /// <summary>
        /// Set the cover image of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <param name="mediaAssetId">
        /// The ID of the media asset
        /// </param>
        /// <returns>
        /// Returns status code 200 if success
        /// </returns>
        /// <response code="204">Update successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to update cover image</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>Agent</c> role.
        /// </remarks>
        [HttpPut("{listingCaseId:int}/cover-image")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(PutResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PutResponse>> SetCoverImageByListingCaseIdAsync(int listingCaseId, [FromQuery] int mediaAssetId)
        {
            await _listingCaseService.SetCoverImageByListingCaseIdAsync(listingCaseId, mediaAssetId);
            
            return StatusCode(204, new PutResponse(true));
        }

        /// <summary>
        /// Get the final selected media assets of a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>
        /// Returns a list of final selected media assets
        /// </returns>
        /// <response code="200">Returns a list of final selected media assets</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to get final selected media assets</response>
        /// <response code="404">Final selected media assets not found</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> and <c>Agent</c> role.
        /// </remarks>
        [HttpGet("{listingCaseId:int}/final-selection")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<IEnumerable<MediaAssetDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetResponse<IEnumerable<MediaAssetDto>>>> GetFinalSelectionByListingCaseIdAsync(int listingCaseId)
        {
            var result = await _listingCaseService.GetFinalSelectionByListingCaseIdAsync(listingCaseId);

            return Ok(new GetResponse<IEnumerable<MediaAssetDto>>(true, result));
        }

        /// <summary>
        /// Select some media files for final property display.
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <param name="setSelectedMediaRequestDto">
        /// The payload containing the list of Ids of selected media assets.
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns status code 200 if success
        /// </returns>
        /// <response code="204">Update successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to set selected media</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>Agent</c> role.
        /// </remarks>
        [HttpPut("{listingCaseId}/selected-media")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(PutResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PutResponse>> SetSelectedMediaByListingCaseIdAsync(
            int listingCaseId, 
            [FromBody] SetSelectedMediaRequestDto setSelectedMediaRequestDto,
            IValidator<SetSelectedMediaRequestDto> validator)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            // Validate
            var validationResult = await validator.ValidateAsync(setSelectedMediaRequestDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                return ValidationProblem(ModelState);
            }

            await _listingCaseService.SetSelectedMediaByListingCaseIdAsync(listingCaseId, setSelectedMediaRequestDto.MediaIds, currentUserId);
        
            return StatusCode(204, new PutResponse(true));
        }

        /// <summary>
        /// Generate shared url for a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns>
        /// Returns the shared url
        /// </returns>
        /// <response code="200">Returns the shared url</response>
        /// <response code="400">Failed to generate shared url</response>
        [HttpPost("{listingCaseId:int}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<string>>> GenerateSharedUrlAsync(int listingCaseId)
        {
            var result = await _listingCaseService.GenerateSharedUrlAsync(listingCaseId);

            return Ok(new PostResponse<string>(true, result));
        }
    }
}
