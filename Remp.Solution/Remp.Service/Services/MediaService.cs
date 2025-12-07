using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILoggerService _loggerService;

    public MediaService(IMediaRepository mediaRepository, IBlobStorageService blobStorageService, ILoggerService loggerService)
    {
        _mediaRepository = mediaRepository;
        _blobStorageService = blobStorageService;
        _loggerService = loggerService;
    }

    public async Task DeleteMediaByIdAsync(int mediaId, string userId)
    {
        // Check if the media exists
        var media = await _mediaRepository.FindMediaByIdAsync(mediaId);
        if (media is null)
        {
            // Log
            await _loggerService.LogDeleteMedia(
                mediaId: null,
                userId: userId,
                error: $"Media {mediaId} does not exist"
                );

            throw new NotFoundException(message: $"Media {mediaId} does not exist", title: "Media does not exist");
        }

        // Check if the user is the owner of the media (PhotographyCompany)
        if (media.UserId != userId)
        {
            // Log
            await _loggerService.LogDeleteMedia(
                mediaId: mediaId.ToString(),
                userId: userId,
                error : $"User is not the owner of this media"
                );

            throw new ForbiddenException(
                message: $"User {userId} cannot delete this media because the user is not the owner of this media",
                title: "You cannot delete this media because you are not the owner of this media"
                );
        }

        await _blobStorageService.DeleteFileAsync(media.MediaUrl);

        await _mediaRepository.DeleteMediaByIdAsync(media);

        // Log
        await _loggerService.LogDeleteMedia(mediaId.ToString(), userId);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadMediaByIdAsync(int mediaAssetId)
    {
        // Check if the media asset exists
        var mediaAsset = await _mediaRepository.FindMediaByIdAsync(mediaAssetId);
        if (mediaAsset is null)
        {
            throw new NotFoundException(message: $"Media asset {mediaAssetId} does not exist", title: "Media asset does not exist");
        }

        var mediaUrl = mediaAsset.MediaUrl;

        return await _blobStorageService.DownloadFileAsync(mediaUrl);
    }
}
