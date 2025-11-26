using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly AppDbContext _dbContext;

    public MediaRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MediaAsset?> FindMediaByIdAsync(int id)
    {
        return await _dbContext.MediaAssets.FindAsync(id);
    }

    public async Task DeleteMediaByIdAsync(MediaAsset mediaAsset)
    {
        _dbContext.MediaAssets.Remove(mediaAsset);
        await _dbContext.SaveChangesAsync();
    }
}
