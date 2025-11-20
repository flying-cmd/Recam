using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class ListingCase
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public int Postcode { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public double FloorArea { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public PropertyType PropertyType { get; set; }
    public SaleCategory SaleCategory { get; set; }
    public ListingCaseStatus ListingCaseStatus { get; set; }

    // Foreign key
    public string UserId { get; set; } = null!;

    // Nagivation properties
    public User User { get; set; } = null!;
    public ICollection<AgentListingCase> AgentListingCases { get; set; } = new List<AgentListingCase>();
    public ICollection<CaseContact> CaseContacts { get; set; } = new List<CaseContact>();
    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();
}
