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
    public class PhotographyCompanyController : ControllerBase
    {
        private readonly IPhotographyCompanyService PhotographyCompanyService;

        public PhotographyCompanyController(IPhotographyCompanyService photographyCompanyService)
        {
            PhotographyCompanyService = photographyCompanyService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AgentResponseDto>>> GetAgentsAsync([FromQuery] int pageNumer, [FromQuery] int pageSize)
        {
            var result = await PhotographyCompanyService.GetAgentsAsync(pageNumer, pageSize);
            return Ok(result);
        }
    }
}
