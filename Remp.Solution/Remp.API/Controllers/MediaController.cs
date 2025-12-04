using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers.ApiResponse;
using Remp.Models.Constants;
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
        /// <response code="204">Media deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to delete media</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> role.
        /// </remarks>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleNames.PhotographyCompany)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DeleteResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<DeleteResponse>> DeleteMediaByIdAsync(int id)
        {
            // Get current user id
            var currentUser = HttpContext.User;
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Forbid();
            }

            await _mediaService.DeleteMediaByIdAsync(id, currentUserId);
            return StatusCode(204, new DeleteResponse(true));
        }

        /// <summary>
        /// Download media by id
        /// </summary>
        /// <param name="mediaAssetId">
        /// The id of the media
        /// </param>
        /// <returns>
        /// The media file
        /// </returns>
        /// <response code="200">Returns the media file</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Failed to download media</response>
        /// <remarks>
        /// This endpoint is restricted to users in the <c>PhotographyCompany</c> or <c>Agent</c> role.
        /// </remarks>
        [HttpGet("download/{mediaAssetId:int}")]
        [Authorize(Roles = $"{RoleNames.PhotographyCompany},{RoleNames.Agent}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DownloadMediaById(int mediaAssetId)
        {
            var (fileStream, contentType, fileName) = await _mediaService.DownloadMediaByIdAsync(mediaAssetId);
            
            return File(fileStream, contentType, fileName);
        }
    }
}
