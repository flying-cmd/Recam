using Microsoft.AspNetCore.Http;

namespace Remp.Service.DTOs;

public class RegisterRequestDto
{
    public string PhotographyCompanyName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
