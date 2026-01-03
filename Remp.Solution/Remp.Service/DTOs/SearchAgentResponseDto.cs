namespace Remp.Service.DTOs;

public class SearchAgentResponseDto
{
    public string Id { get; set; } = null!;
    public string AgentFirstName { get; set; } = null!;
    public string AgentLastName { get; set; } = null!;
    public string AvataUrl { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
}
