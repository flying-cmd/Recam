using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IMediaService
{
    Task<DeleteMediaResponseDto> DeleteMediaByIdAsync(int mediaId, string userId);
}
