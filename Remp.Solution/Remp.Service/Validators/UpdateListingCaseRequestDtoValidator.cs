using FluentValidation;
using Remp.Models.Enums;
using Remp.Service.DTOs;

namespace Remp.Service.Validators;

public class UpdateListingCaseRequestDtoValidator : AbstractValidator<UpdateListingCaseRequestDto>
{
    public UpdateListingCaseRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255).When(x => x.Title != null).WithMessage("Title must be at most 255 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => x.Description != null).WithMessage("Description must be at most 1000 characters long.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(50).When(x => x.Street != null).WithMessage("Street must be at most 50 characters long.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(20).When(x => x.City != null).WithMessage("City must be at most 20 characters long.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(20).When(x => x.State != null).WithMessage("State must be at most 20 characters long.");

        RuleFor(x => x.Postcode)
            .NotEmpty().When(x => x.Postcode != null).WithMessage("Postcode is required.");

        RuleFor(x => x.Longitude)
            .NotEmpty().When(x => x.Longitude != null).WithMessage("Longitude is required.");

        RuleFor(x => x.Latitude)
            .NotEmpty().When(x => x.Latitude != null).WithMessage("Latitude is required.");

        RuleFor(x => x.Price)
            .NotEmpty().When(x => x.Price != null).WithMessage("Price is required.")
            .GreaterThan(0).When(x => x.Price != null).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Bedrooms)
            .NotEmpty().When(x => x.Bedrooms != null).WithMessage("Bedrooms is required.")
            .GreaterThanOrEqualTo(0).When(x => x.Bedrooms != null).WithMessage("Bedrooms must be greater than 0.");

        RuleFor(x => x.Bathrooms)
            .NotEmpty().When(x => x.Bathrooms != null).WithMessage("Bathrooms is required.")
            .GreaterThanOrEqualTo(0).When(x => x.Bathrooms != null).WithMessage("Bathrooms must be greater than 0.");

        RuleFor(x => x.Garages)
            .NotEmpty().When(x => x.Garages != null).WithMessage("Garages is required.")
            .GreaterThanOrEqualTo(0).When(x => x.Garages != null).WithMessage("Garages must be greater than 0.");

        RuleFor(x => x.FloorArea)
            .NotEmpty().When(x => x.FloorArea != null).WithMessage("Floor area is required.")
            .GreaterThanOrEqualTo(0).When(x => x.FloorArea != null).WithMessage("Floor area must be greater than 0.");

        RuleFor(x => x.PropertyType)
            .NotEmpty().When(x => x.PropertyType != null).WithMessage("Property type is required.")
            .IsEnumName(typeof(PropertyType)).When(x => x.PropertyType != null).WithMessage("Invalid property type.");

        RuleFor(x => x.SaleCategory)
            .NotEmpty().When(x => x.SaleCategory != null).WithMessage("Sale category is required.")
            .IsEnumName(typeof(SaleCategory)).When(x => x.SaleCategory != null).WithMessage("Invalid sale category.");
    }
}
