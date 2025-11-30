using FluentValidation;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateMediaRequestDtoValidator : AbstractValidator<CreateMediaRequestDto>
{
    public CreateMediaRequestDtoValidator()
    {
        RuleFor(x => x.MediaFiles)
            .NotEmpty().WithMessage("Media files are required.");

        RuleFor(x => x.MediaType)
            .NotEmpty().WithMessage("Media type is required.")
            .IsEnumName(typeof(MediaType)).WithMessage("Invalid media type.");
    }
}
