using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateCaseContactRequestDtoValidator : AbstractValidator<CreateCaseContactRequestDto>
{
    public CreateCaseContactRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(20).WithMessage("First name must be at most 20 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(20).WithMessage("Last name must be at most 20 characters long.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(20).WithMessage("Company name must be at most 20 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");
    }
}
