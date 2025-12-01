using Microsoft.EntityFrameworkCore;
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
        return await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteMediaByIdAsync(MediaAsset mediaAsset)
    {
        _dbContext.MediaAssets.Remove(mediaAsset);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMediaAssetsAsync(IEnumerable<MediaAsset> MediaAssets)
    {
        _dbContext.MediaAssets.UpdateRange(MediaAssets);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<MediaAsset>> FindMediaByIdsAsync(IEnumerable<int> mediaIds)
    {
        return await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(m => mediaIds.Contains(m.Id))
            .ToListAsync();
    }
}
