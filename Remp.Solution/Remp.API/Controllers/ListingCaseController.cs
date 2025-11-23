using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingCaseController : ControllerBase
    {
        private readonly IListingCaseService _listingCaseService;

        public ListingCaseController(IListingCaseService listingCaseService)
        {
            _listingCaseService = listingCaseService;
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        public async Task<ActionResult<CreateListingCaseResponseDto>> CreateListingCase(
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

                return ValidationProblem(problemDetails);
            }

            var result = await _listingCaseService.CreateListingCaseAsync(createListingCaseRequest);
            
            // TODO: later change to location with id
            return Ok(result);
        }
    }
}
