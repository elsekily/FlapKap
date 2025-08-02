using FlapKap.Application.DTOs.Product;
using FluentValidation;

namespace FlapKap.Application.Validators.Product;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(p => p.Id).NotEmpty();

        RuleFor(p => p.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(p => p.AmountAvailable)
            .GreaterThanOrEqualTo(0);

        RuleFor(p => p.Cost)
            .GreaterThan(0);
    }
}