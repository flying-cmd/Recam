namespace Remp.Service.DTOs;

// Used as Response
public class CreateAgentAccountResponseDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; private set; } = null!;

    public CreateAgentAccountResponseDto(string id, string email, string password)
    {
        Id = id;
        Email = email;
        Password = password;
    }
}
