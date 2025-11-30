namespace Remp.Service.DTOs;

public class CreateCaseContactRequestDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ProfileUrl { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int listingCaseId { get; set; }
}
