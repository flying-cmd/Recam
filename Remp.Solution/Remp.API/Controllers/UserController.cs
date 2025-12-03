using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
using Remp.Common.Helpers.ApiResponse;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;
using System.Security.Claims;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all agents in pagination.
        /// </summary>
        /// <param name="pageNumer">
        /// Page number
        /// </param>
        /// <param name="pageSize">
        /// Page size (how many items per page)
        /// </param>
        /// <returns>
        /// Returns a list of agents, current page number, page size, total pages and total count.
        /// </returns>
        /// <response code="200">Returns a list of agents</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Invalid page number or page size</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<PagedResult<CreateAgentAccountResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<GetResponse<PagedResult<CreateAgentAccountResponseDto>>>> GetAgentsAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
        {
            var result = await _userService.GetAgentsAsync(pageNumer, pageSize);
            return Ok(new GetResponse<PagedResult<CreateAgentAccountResponseDto>>(true, result));
        }

        /// <summary>
        /// Get current user information including user Id, role and listing case Ids which the user has.
        /// </summary>
        /// <returns>
        /// Returns current user information including user Id, role and listing case Ids which the user has.
        /// </returns>
        /// <response code="200">Returns current user information including user Id, role and listing case Ids which the user has.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to get current user information</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>Agent</c> role.
        /// </remarks>
        [HttpGet("~/api/users/me")]
        [Authorize(Roles = RoleNames.Agent)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<UserInfoResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<GetResponse<UserInfoResponseDto>>> GetCurrentUserInfoAsync()
        {
            // Get current user
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

            var userInfoResponseDto = new UserInfoResponseDto();
            userInfoResponseDto.Id = currentUserId;
            userInfoResponseDto.Role = currrentUserRole;

            var userListingCaseIds = await _userService.GetUserListingCaseIdsAsync(currentUserId);
            userInfoResponseDto.ListingCaseIds = userListingCaseIds;

            return Ok(new GetResponse<UserInfoResponseDto>(true, userInfoResponseDto));
        }

        /// <summary>
        /// Photograhpy company adds an agent by agent id.
        /// </summary>
        /// <param name="agentId">
        /// The ID of the agent to add.
        /// </param>
        /// <returns>
        /// Returns status code 200 if success. Returns status code 400 if failed.
        /// </returns>
        /// <response code="200">Agent added</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to add agent or agent has already been added</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpPut("photography-company/agent/{agentId}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PutResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PutResponse>> AddAgentByIdAsync(string agentId)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = await _userService.AddAgentByIdAsync(agentId, currentUserId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(new PutResponse(true));
        }

        /// <summary>
        /// Photography company creates an agent account.
        /// </summary>
        /// <param name="createAgentAccountRequestDto">
        /// The payload containing the details (The email) of the agent to create.</param>
        /// <param name="validator"></param>
        /// <param name="emailService"></param>
        /// <param name="configuration"></param>
        /// <returns>
        /// If success, returns status code 200 and a message indicating that the agent account has been created.
        /// </returns>
        /// <response code="200">Agent account created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to create agent account</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpPost("agent")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse<string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<string>>> CreateAgentAccountAsync(
            [FromBody] CreateAgentAccountRequestDto createAgentAccountRequestDto,
            IValidator<CreateAgentAccountRequestDto> validator,
            [FromServices] IEmailService emailService,
            [FromServices] IConfiguration configuration)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(createAgentAccountRequestDto);
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

            var result = await _userService.CreateAgentAccountAsync(createAgentAccountRequestDto, currentUserId);

            if (result != null)
            {
                // Send email
                var emailBody = EmailTemplates.CreateAccountEmail(
                    result.Password,
                    result.Email,
                    configuration["Url:LoginUrl"]!
                    );

                await emailService.SendEmailAsync(
                    createAgentAccountRequestDto.Email,
                    "Account created successfully",
                    emailBody
                );

                return Ok(new PostResponse<string>(true, result.Email));
            }

            return BadRequest();
        }

        /// <summary>
        /// Search agent by email.
        /// </summary>
        /// <param name="searchAgentRequestDto"></param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the agent.
        /// </returns>
        /// <response code="200">Returns details information of the agent</response>
        /// <response code="404">Not found the agent</response>
        /// <response code="400">Invalid email</response>
        [HttpGet("agent/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<SearchAgentResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<GetResponse<SearchAgentResponseDto>>> GetAgentByEmailAsync(
            [FromQuery] SearchAgentRequestDto searchAgentRequestDto,
            IValidator<SearchAgentRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(searchAgentRequestDto);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                return ValidationProblem(problemDetails);
            }

            var result = await _userService.GetAgentByEmailAsync(searchAgentRequestDto.Email);
            
            if (result != null)
            {
                return Ok(new GetResponse<SearchAgentResponseDto>(true, result));
            }

            return NotFound();
        }


        /// <summary>
        /// Get agents under photography company
        /// </summary>
        /// <returns>
        /// Returns a list of agents under the photography company
        /// </returns>
        /// <response code="200">Returns a list of agents under the photography company</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to get agents under photography company</response>
        [HttpGet("agentlists")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResponse<IEnumerable<SearchAgentResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<GetResponse<IEnumerable<SearchAgentResponseDto>>>> GetAgentsUnderPhotographyCompanyAsync()
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = await _userService.GetAgentsUnderPhotographyCompanyAsync(currentUserId);

            return Ok(new GetResponse<IEnumerable<SearchAgentResponseDto>>(true, result));
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="updatePasswordRequestDto">
        /// The payload containing the old password, new password and confirm password of the user.
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns a message indicating whether the password is updated successfully or not.
        /// </returns>
        /// <response code="200">Returns a message indicating the password is updated successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to update password</response>
        [HttpPut("password")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<UpdateApiResponse>> UpdatePasswordAsync([FromBody] UpdatePasswordRequestDto updatePasswordRequestDto, IValidator<UpdatePasswordRequestDto> validator)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            // Validate
            var validationResult = await validator.ValidateAsync(updatePasswordRequestDto);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                // Log
                UserActivityLog.LogUpdatePassword(
                    userId: currentUserId,
                    description: $"User {currentUserId} failed to update password because of the errors: {errors}");

                return ValidationProblem(problemDetails);
            }

            var result = await _userService.UpdatePasswordAsync(updatePasswordRequestDto, currentUserId);
            
            if (result.Success)
            {
                // Log
                UserActivityLog.LogUpdatePassword(
                    userId: currentUserId);

                return Ok(new PutResponse(true));
            }

            // Log
            UserActivityLog.LogUpdatePassword(
                userId: currentUserId,
                description: $"User {currentUserId} failed to update password because of the errors: {result.Error}");

            return BadRequest();
        }
    }
}
