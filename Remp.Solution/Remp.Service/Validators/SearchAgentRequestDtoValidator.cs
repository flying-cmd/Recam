using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class SearchAgentRequestDtoValidator : AbstractValidator<SearchAgentRequestDto>
{
    public SearchAgentRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}
