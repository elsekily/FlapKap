using FlapKap.Application.DTOs.User;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using FlapKap.Domain.Constants;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FlapKap.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly ITokenBlacklistService tokenBlacklistService;
    private readonly IValidator<CreateUserDto> createUservalidator;
    private readonly IValidator<UpdateUserDto> updateUservalidator;

    public UserService(IUnitOfWork unitOfWork,
                       UserManager<ApplicationUser> userManager,
                       RoleManager<ApplicationRole> roleManager,
                       ITokenBlacklistService tokenBlacklistService,
                       IValidator<CreateUserDto> createUservalidator,
                       IValidator<UpdateUserDto> updateUservalidator)
    {
        this.unitOfWork = unitOfWork;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.tokenBlacklistService = tokenBlacklistService;
        this.createUservalidator = createUservalidator;
        this.updateUservalidator = updateUservalidator;
    }

    public async Task<Result> CreateUserAsync(CreateUserDto createUserDto, string role = RoleConstants.Buyer)
    {
        var validationResult = createUservalidator.Validate(createUserDto);
        if (!validationResult.IsValid)
            return Result<ValidationResult>.Failure("Please Validate This Data!", validationResult);

        var user = createUserDto.Adapt<ApplicationUser>();

        var result = await userManager.CreateAsync(user, createUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<UserResponseDto>.Failure(errors);
        }
        await userManager.AddToRolesAsync(user, [role]);
        var createdUser = await GetUserByUsernameAsync(user.UserName);

        if (!createdUser.IsSuccess)
            return Result<UserResponseDto>.Failure("Something went wrong!");
        
        return createdUser;
    }

    public async Task<Result> UpdateUserAsync(string username, UpdateUserDto updateUserDto)
    {
        var validationResult = updateUservalidator.Validate(updateUserDto);
        if (!validationResult.IsValid)
            return Result<ValidationResult>.Failure("Please Validate This Data!",validationResult);


        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            return Result<UserResponseDto>.Failure("User not found");

        updateUserDto.Adapt(user);
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<UserResponseDto>.Failure(errors);
        }
        
        var updatedUser = await GetUserByUsernameAsync(user.UserName);
        return updatedUser;
    }

    public async Task<Result> DeleteUserAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            return Result.Failure("User not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Failure(errors);
        }
        
        tokenBlacklistService.BlacklistToken();
        return Result.Success();
    }

    public async Task<Result<UserResponseDto>> GetUserByUsernameAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);
        
        if (user == null)
            return Result<UserResponseDto>.Failure("User not found");

        var userDto = user.Adapt<UserResponseDto>();
        var roles = await userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();


        return Result<UserResponseDto>.Success(userDto);
    }
}