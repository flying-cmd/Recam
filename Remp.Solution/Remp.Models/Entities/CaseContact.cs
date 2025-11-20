namespace Remp.Models.Entities;

public class CaseContact
{
    // Primary Key
    public int ContactId { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ProfileUrl { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    // Foreign Key
    public int ListingCaseId { get; set; }

    // Navigation Properties
    public ListingCase ListingCase { get; set; } = null!;
}
