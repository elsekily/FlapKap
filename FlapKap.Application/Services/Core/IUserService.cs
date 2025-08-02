using FlapKap.Application.DTOs.User;
using FlapKap.Domain.Common;
using FlapKap.Domain.Constants;

namespace FlapKap.Application.Services.Core;

public interface IUserService
{
    Task<Result> CreateUserAsync(CreateUserDto createUserDto, string role = RoleConstants.Buyer);
    Task<Result> UpdateUserAsync(string username, UpdateUserDto updateUserDto);
    Task<Result> DeleteUserAsync(string username);
    Task<Result<UserResponseDto>> GetUserByUsernameAsync(string username);
}