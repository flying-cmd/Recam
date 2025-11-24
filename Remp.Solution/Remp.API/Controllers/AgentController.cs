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
    [Authorize(Roles = RoleNames.PhotographyCompany)]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AgentResponseDto>>> GetAgentsAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
        {
            var result = await _agentService.GetAgentsAsync(pageNumer, pageSize);
            return Ok(result);
        }
    }
}
