namespace Remp.Models.Entities;

public class AgentListingCase
{
    // Foreign keys
    public string AgentId { get; set; } = null!;
    public int ListingCaseId { get; set; }

    // Navigation properties
    public Agent Agent { get; set; } = null!;
    public ListingCase ListingCase { get; set; } = null!;
}
