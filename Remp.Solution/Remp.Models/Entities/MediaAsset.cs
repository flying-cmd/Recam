using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class MediaAsset
{
    // Primary Key
    public int Id { get; set; }

    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public bool IsSelect { get; set; }
    public bool IsHero { get; set; }
    public bool IsDeleted { get; set; }

    // Foreign Keys
    public int ListingCaseId { get; set; }
    public string UserId { get; set; } = null!;

    // Navigation Properties
    public ListingCase ListingCase { get; set; } = null!;
    public User User { get; set; } = null!;
}
