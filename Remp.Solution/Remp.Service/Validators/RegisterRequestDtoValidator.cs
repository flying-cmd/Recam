using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    private static readonly string[] AllowedFileTypes = { "image/jpeg", "image/jpg", "image/png" };
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(20).WithMessage("First name must be at most 20 characters long.");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(20).WithMessage("Last name must be at most 20 characters long.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(20).WithMessage("Company name must be at most 20 characters long.");

        RuleFor(x => x.Avatar)
            .NotNull().WithMessage("Avatar is required.")
            .Must(f => AllowedFileTypes.Contains(f.ContentType)).WithMessage("Avatar must be a JPEG, JPG or PNG image.");
    }
}
