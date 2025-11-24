using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class MediaAssetDto
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public bool IsSelect { get; set; }
    public bool IsHero { get; set; }
    public bool IsDeleted { get; set; }
}
