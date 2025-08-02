using FlapKap.Application.DTOs.Transaction;
using FluentValidation;

namespace FlapKap.Application.Validators.Transaction;

public class BuyRequestDtoValidator : AbstractValidator<BuyRequestDto>
{
    public BuyRequestDtoValidator()
    {
        RuleFor(x => x.Products)
            .NotNull().WithMessage("Products list is required.")
            .NotEmpty().WithMessage("Products list cannot be empty.");

        RuleForEach(x => x.Products)
            .SetValidator(new BuyProductDtoValidator());
    }
}