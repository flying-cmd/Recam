namespace Remp.Service.DTOs;

public class ListingCaseResponseDto
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
    public string PropertyType { get; set; } = null!;
    public string SaleCategory { get; set; } = null!;
    public string ListingCaseStatus { get; set; } = null!;
    public string SharedUrl { get; set; } = string.Empty;
    public IEnumerable<AgentDto> Agents { get; set; } = new List<AgentDto>();
}
