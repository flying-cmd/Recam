using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class SetSelectedMediaRequestDtoValidator : AbstractValidator<SetSelectedMediaRequestDto>
{
    public SetSelectedMediaRequestDtoValidator()
    {
        RuleFor(x => x.MediaIds)
            .NotEmpty().WithMessage("Media ID is required.");
    }
}
