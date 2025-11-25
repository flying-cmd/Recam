using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
using Remp.Models.Constants;
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

                // TODO: Log
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
    }
}
