using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
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

        public ListingCaseController(IListingCaseService listingCaseService)
        {
            _listingCaseService = listingCaseService;
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
        public async Task<ActionResult<ListingCaseResponseDto>> GetListingCaseByListingCaseIdAsync(int listingCaseId)
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
            return Ok(result);
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
        public async Task<ActionResult<ListingCaseResponseDto>> CreateListingCase(
            [FromBody] CreateListingCaseRequestDto createListingCaseRequest, 
            IValidator<CreateListingCaseRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createListingCaseRequest);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                // Log
                CaseHistoryLog.LogCreateListingCase(
                    listingCaseId: null,
                    userId: createListingCaseRequest.UserId,
                    description: $"User failed to create listing case with erros: {errors}"
                );

                return ValidationProblem(problemDetails);
            }

            var result = await _listingCaseService.CreateListingCaseAsync(createListingCaseRequest);
            
            return CreatedAtRoute(nameof(GetListingCaseByListingCaseIdAsync), new { listingCaseId = result.Id }, result);
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
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> or <c>Agent</c> roles.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        public async Task<ActionResult<IEnumerable<ListingCaseResponseDto>>> GetAllListingCasesAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
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

            return Ok(result);
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
        public async Task<IActionResult> UpdateListingCaseAsync(int listingCaseId, [FromBody] UpdateListingCaseRequestDto updateListingCaseRequest, IValidator<UpdateListingCaseRequestDto> validator)
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
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                // Log
                CaseHistoryLog.LogUpdateListingCase(
                    listingCaseId: listingCaseId.ToString(),
                    userId: currentUserId,
                    updatedFields: null,
                    description: $"User failed to update listing case with erros: {errors}"
                );

                return ValidationProblem(problemDetails);
            }

            await _listingCaseService.UpdateListingCaseAsync(listingCaseId, updateListingCaseRequest, currentUserId);
            return NoContent();
        }

        /// <summary>
        /// Delete a listing case
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case to delete
        /// </param>
        /// <returns>If success, returns a message "Listing case deleted successfully."</returns>
        /// <response code="200">Listing case deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to delete listing case</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpDelete("{listingCaseId:int}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        public async Task<ActionResult<DeleteListingCaseResponseDto>> DeleteListingCaseByListingCaseIdAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.DeleteListingCaseByListingCaseIdAsync(listingCaseId, currentUserId);

            return Ok(result);
        }

        /// <summary>
        /// Update listing case status
        /// </summary>
        /// <param name="listingCaseId">
        /// The ID of the listing case
        /// </param>
        /// <returns></returns>
        /// <response code="204">Listing case status updated</response>
        /// <response code="400">Failed to update listing case status</response>
        /// <remarks>
        /// Updating listing case status must follow the workflow of the listing case (Created -> Pending -> Delivered).
        /// </remarks>
        [HttpPatch("{listingCaseId:int}/status")]
        public async Task<IActionResult> UpdateListingCaseStatusAsync(int listingCaseId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            await _listingCaseService.UpdateListingCaseStatusAsync(listingCaseId, currentUserId);
            return NoContent();
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
        /// <remarks>
        /// This endpoint is restricted to the phtography companiey who created the listing case and the agent who is assigned the listing case.
        /// </remarks>
        [HttpGet("{listingCaseId:int}/media", Name = "GetListingCaseMediaByListingCaseIdAsync")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        public async Task<ActionResult<IEnumerable<MediaAssetDto>>> GetListingCaseMediaByListingCaseIdAsync(int listingCaseId)
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
            return Ok(result);
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
        /// <remarks>
        /// This endpoint is restricted to the phtography companiey who created the listing case and the agent who is assigned the listing case.
        /// </remarks>
        [HttpGet("{listingCaseId:int}/contact", Name = "GetListingCaseContactByListingCaseIdAsync")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        public async Task<ActionResult<IEnumerable<CaseContactDto>>> GetListingCaseContactByListingCaseIdAsync(int listingCaseId)
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
            return Ok(result);
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
        public async Task<ActionResult<CaseContactDto>> CreateCaseContactByListingCaseIdAsync(
            int listingCaseId, 
            [FromBody] CreateCaseContactRequestDto createCaseContactRequest,
            IValidator<CreateCaseContactRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createCaseContactRequest);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));
                
                return ValidationProblem(problemDetails);
            }

            var result = await _listingCaseService.CreateCaseContactByListingCaseIdAsync(listingCaseId, createCaseContactRequest);
            
            return CreatedAtRoute(nameof(GetListingCaseContactByListingCaseIdAsync), new { listingCaseId }, result);
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
        public async Task<ActionResult<MediaAssetDto>> CreateMediaByListingCaseIdAsync(
            [FromForm] CreateMediaRequestDto createMediaRequestDto,
            int listingCaseId,
            IValidator<CreateMediaRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createMediaRequestDto);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                return ValidationProblem(problemDetails);
            }

            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = await _listingCaseService.CreateMediaByListingCaseIdAsync(createMediaRequestDto.MediaFiles, (MediaType)Enum.Parse(typeof(MediaType), createMediaRequestDto.MediaType), listingCaseId, currentUserId);
            return CreatedAtRoute(nameof(GetListingCaseMediaByListingCaseIdAsync), new { listingCaseId }, result);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAllMediaByListingCaseIdAsync(int listingCaseId)
        {
            var (zipStream, contentType, fileName) = await _listingCaseService.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

            return File(zipStream, contentType, fileName);
        }
    }
}
