using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateAgentAccountRequestDtoValidator : AbstractValidator<CreateAgentAccountRequestDto>
{
    public CreateAgentAccountRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}
