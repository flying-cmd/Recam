using Microsoft.AspNetCore.Http;

namespace Remp.Service.DTOs;

public class CreateAgentAccountRequestDto
{
    public string Email { get; set; } = null!;
    public string AgentFirstName { get; set; } = null!;
    public string AgentLastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public IFormFile? Avatar { get; set; }
}
