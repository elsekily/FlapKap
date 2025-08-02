using FlapKap.Application.DTOs.Auth;
using FluentValidation;

namespace FlapKap.Application.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserName)
           .NotEmpty().WithMessage("Username is required")
           .Matches("^[a-zA-Z0-9]*$").MaximumLength(50).WithMessage("Only letters and digits are allowed, max length is 50.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
} 