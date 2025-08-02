
using FlapKap.Application.DTOs.Transaction;
using FluentValidation;

namespace FlapKap.Application.Validators.Transaction;

public class BuyProductDtoValidator : AbstractValidator<BuyProductDto>
{
    public BuyProductDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Invalid product ID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}