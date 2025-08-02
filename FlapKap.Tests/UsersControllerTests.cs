using FlapKap.API.Controllers;
using FlapKap.Application.DTOs.User;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using FlapKap.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlapKap.Tests;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly Mock<IUserService> _serviceMock;

    public UsersControllerTests()
    {
        _serviceMock = new Mock<IUserService>();
        _controller = new UsersController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetUserByUsernameAsync("john"))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto()));

        var result = await _controller.GetUserByUserName("john");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetUserByUsernameAsync("missing"))
            .ReturnsAsync(Result<UserResponseDto>.Failure("Not found"));

        var result = await _controller.GetUserByUserName("missing");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateBuyer_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>(), RoleConstants.Buyer))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto()));

        var result = await _controller.CreateBuyer(new CreateUserDto());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CreateBuyer_ReturnsBadRequest_WhenFailure()
    {
        _serviceMock.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>(), RoleConstants.Buyer))
            .ReturnsAsync(Result.Failure("Invalid"));

        var result = await _controller.CreateBuyer(new CreateUserDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateSeller_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>(), RoleConstants.Seller))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto()));

        var result = await _controller.CreateSeller(new CreateUserDto());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.UpdateUserAsync("john", It.IsAny<UpdateUserDto>()))
            .ReturnsAsync(Result<UserResponseDto>.Success(new UserResponseDto()));

        var result = await _controller.UpdateUser("john", new UpdateUserDto());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenFailure()
    {
        _serviceMock.Setup(s => s.UpdateUserAsync("john", It.IsAny<UpdateUserDto>()))
            .ReturnsAsync(Result.Failure("Invalid"));

        var result = await _controller.UpdateUser("john", new UpdateUserDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenSuccess()
    {
        _serviceMock.Setup(s => s.DeleteUserAsync("john"))
            .ReturnsAsync(Result.Success());

        var result = await _controller.DeleteUser("john");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.DeleteUserAsync("john"))
            .ReturnsAsync(Result.Failure("Not found"));

        var result = await _controller.DeleteUser("john");

        Assert.IsType<NotFoundObjectResult>(result);
    }
}