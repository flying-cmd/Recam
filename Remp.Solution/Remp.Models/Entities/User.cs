using Microsoft.AspNetCore.Identity;

namespace Remp.Models.Entities;

public class User : IdentityUser
{
    // Navigation Properties
    public Agent Agent { get; set; } = null!;
    public PhotographyCompany PhotographyCompany { get; set; } = null!;
    public ICollection<ListingCase> ListingCases { get; set; } = new List<ListingCase>();
    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();
}
