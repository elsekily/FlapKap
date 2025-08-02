using FlapKap.API.Controllers;
using FlapKap.Application.DTOs.Auth;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlapKap.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenSuccess()
    {
        var loginDto = new LoginDto { UserName = "test", Password = "pass" };
        var tokenResult = Result<TokenResponseDto>.Success(new TokenResponseDto { Token = "mocked-jwt" });

        _authServiceMock.Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(tokenResult);

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tokenResult, okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenInvalid()
    {
        var loginDto = new LoginDto { UserName = "invalid", Password = "wrong" };
        var failedResult = Result<TokenResponseDto>.Failure("Invalid username or password");

        _authServiceMock.Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(failedResult);

        var result = await _controller.Login(loginDto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(failedResult, badRequest.Value);
    }
}
