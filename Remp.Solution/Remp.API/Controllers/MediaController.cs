using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Security.Claims;

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// Delete media by id
        /// </summary>
        /// <param name="id">
        /// The id of the media
        /// </param>
        /// <returns>
        /// If success, returns a message "Media deleted successfully."
        /// </returns>
        /// <response code="200">Media deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to delete media</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        public async Task<ActionResult<DeleteMediaResponseDto>> DeleteMediaByIdAsync(int id)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            return await _mediaService.DeleteMediaByIdAsync(id, currentUserId);
        }
    }
}
