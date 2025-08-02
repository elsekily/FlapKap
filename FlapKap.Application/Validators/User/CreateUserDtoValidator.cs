using FlapKap.Application.DTOs.User;
using FluentValidation;

namespace FlapKap.Application.Validators.User;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Matches("^[a-zA-Z0-9]*$").MaximumLength(50).WithMessage("Only letters and digits are allowed, max length is 50.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\d+$").WithMessage("Phone number is numbers only");
    }
} 