using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _mediaRepository;

    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<DeleteMediaResponseDto> DeleteMediaByIdAsync(int mediaId, string userId)
    {
        // Check if the media exists
        var media = await _mediaRepository.FindMediaByIdAsync(mediaId);
        if (media is null)
        {
            // Log
            CaseHistoryLog.LogDeleteMedia(
                mediaId: null,
                userId: userId,
                description: $"User {userId} try to delete media {mediaId}, but it does not exist"
                );

            throw new NotFoundException(message: $"Media {mediaId} does not exist", title: "Media does not exist");
        }

        // Check if the user is the owner of the media (PhotographyCompany)
        if (media.UserId != userId)
        {
            // Log
            CaseHistoryLog.LogDeleteMedia(
                mediaId: mediaId.ToString(),
                userId: userId,
                description: $"User {userId} cannot delete the media {mediaId} because the user is not the owner of this media"
                );

            throw new UnauthorizedException(
                message: $"User {userId} cannot delete this media because the user is not the owner of this media",
                title: "You cannot delete this media because you are not the owner of this media"
                );
        }

        try
        {
            await _mediaRepository.DeleteMediaByIdAsync(media);
        }
        catch (Exception ex)
        {
            // Log
            CaseHistoryLog.LogDeleteMedia(
                mediaId: mediaId.ToString(),
                userId: userId,
                description: $"User {userId} failed to delete media {mediaId} because of error: {ex.Message}"
                );

            throw new DeleteException(message: ex.Message, title: "Failed to delete media.");
        }

        // Log
        CaseHistoryLog.LogDeleteMedia(mediaId.ToString(), userId);

        return new DeleteMediaResponseDto();
    }
}
