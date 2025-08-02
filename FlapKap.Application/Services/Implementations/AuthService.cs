using FlapKap.Application.DTOs.Auth;
using FlapKap.Application.DTOs.User;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace FlapKap.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IJwtService jwtService;
    private readonly IValidator<LoginDto> loginValidator;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, IValidator<LoginDto> loginValidator)
    {
        this.userManager = userManager;
        this.jwtService = jwtService;
        this.loginValidator = loginValidator;
    }

    public async Task<Result> LoginAsync(LoginDto loginDto)
    {
        var validationResult = loginValidator.Validate(loginDto);
        if (!validationResult.IsValid)
            return Result<ValidationResult>.Failure("Please validate", validationResult);

        var user = await userManager.FindByNameAsync(loginDto.UserName);
        
        if (user == null || !(await userManager.CheckPasswordAsync(user, loginDto.Password)))
        {
            return Result<TokenResponseDto>.Failure("Invalid username or password");
        }

        return await jwtService.GenerateTokenAsync(user);
    }
}