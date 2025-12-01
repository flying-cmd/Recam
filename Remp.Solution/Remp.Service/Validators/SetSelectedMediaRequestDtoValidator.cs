using FluentValidation;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class SetSelectedMediaRequestDtoValidator : AbstractValidator<SetSelectedMediaRequestDto>
{
    public SetSelectedMediaRequestDtoValidator()
    {
        RuleFor(x => x.MediaIds)
            .NotEmpty().WithMessage("Media ID is required.")
            .Must(x => x.Count() <= 10).WithMessage("A maximum of 10 media files can be selected.");
    }
}
