using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _agentService;

        public UserController(IUserService agentService)
        {
            _agentService = agentService;
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
            var result = await _agentService.GetAgentsAsync(pageNumer, pageSize);
            return Ok(result);
        }
    }
}
