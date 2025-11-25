using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
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
        public async Task<ActionResult<PagedResult<AgentResponseDto>>> GetAgentsAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
        {
            var result = await _userService.GetAgentsAsync(pageNumer, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get current user information including user Id, role and listing case Ids which the user has.
        /// </summary>
        /// <returns>
        /// Returns current user information including user Id, role and listing case Ids which the user has.
        /// </returns>
        /// <response code="200">Returns current user information including user Id, role and listing case Ids which the user has.</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>Agent</c> role.
        /// </remarks>
        [HttpGet("~/api/users/me")]
        [Authorize(Roles = RoleNames.Agent)]
        public async Task<ActionResult<UserInfoResponseDto>> GetCurrentUserInfoAsync()
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

            return Ok(userInfoResponseDto);
        }
    }
}
