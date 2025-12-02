using Microsoft.AspNetCore.Http;

namespace Remp.Service.DTOs;

public class RegisterRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public IFormFile Avatar { get; set; } = null!;
}
