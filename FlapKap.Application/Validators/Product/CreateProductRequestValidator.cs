using FlapKap.Application.DTOs.Product;
using FluentValidation;

namespace FlapKap.Application.Validators.Product;

public class CreateProductRequestValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(p => p.AmountAvailable)
            .GreaterThanOrEqualTo(0);

        RuleFor(p => p.Cost)
            .GreaterThan(0);
    }
}