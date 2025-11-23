using FluentValidation;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class CreateListingCaseRequestDtoValidator : AbstractValidator<CreateListingCaseRequestDto>
{
    public CreateListingCaseRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title must be at most 255 characters long.");
    
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must be at most 1000 characters long.");
    
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(50).WithMessage("Street must be at most 50 characters long.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(20).WithMessage("City must be at most 20 characters long.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(20).WithMessage("State must be at most 20 characters long.");

        RuleFor(x => x.Postcode)
            .NotEmpty().WithMessage("Postcode is required.");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude is required.");

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude is required.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Bedrooms)
            .NotEmpty().WithMessage("Bedrooms is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Bedrooms must be greater than 0.");
    
        RuleFor(x => x.Bathrooms)
            .NotEmpty().WithMessage("Bathrooms is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be greater than 0.");
    
        RuleFor(x => x.Garages)
            .NotEmpty().WithMessage("Garages is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Garages must be greater than 0.");

        RuleFor(x => x.FloorArea)
            .NotEmpty().WithMessage("Floor area is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Floor area must be greater than 0.");

        RuleFor(x => x.PropertyType)
            .NotEmpty().WithMessage("Property type is required.")
            .IsEnumName(typeof(PropertyType)).WithMessage("Invalid property type.");

        RuleFor(x => x.SaleCategory)
            .NotEmpty().WithMessage("Sale category is required.")
            .IsEnumName(typeof(SaleCategory)).WithMessage("Invalid sale category.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
