using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IMediaRepository
{
    Task<MediaAsset?> FindMediaByIdAsync(int id);
    Task DeleteMediaByIdAsync(MediaAsset mediaAsset);
}
