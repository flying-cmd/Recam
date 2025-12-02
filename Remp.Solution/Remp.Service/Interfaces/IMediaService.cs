using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IMediaService
{
    Task DeleteMediaByIdAsync(int mediaId, string userId);
    Task<(Stream FileStream, string ContentType, string FileName)> DownloadMediaByIdAsync(int mediaAssetId);
}
