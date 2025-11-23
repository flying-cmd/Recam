namespace Remp.Service.DTOs;

// Used as Response
public class AgentResponseDto
{
    public string Email { get; set; } = null!;
    public string AgentFirstName { get; set; } = null!;
    public string AgentLastName { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
}
