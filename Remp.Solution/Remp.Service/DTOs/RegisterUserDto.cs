using Microsoft.AspNetCore.Http;

namespace Remp.Service.DTOs;

public class RegisterUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;

    public RegisterUserDto(string email, string password, string confirmPassword, string firstName, string lastName, string companyName, string avatarUrl)
    {
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        AvatarUrl = avatarUrl;
    }
}
