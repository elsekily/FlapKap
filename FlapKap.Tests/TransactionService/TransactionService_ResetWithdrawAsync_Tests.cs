using FlapKap.Application.DTOs.Transaction;
using FlapKap.Application.Enums;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using Moq;

namespace FlapKap.Tests.TransactionService;

public class TransactionService_ResetWithdrawAsync_Tests
{
    private readonly Mock<IValidator<DepositCoinsDto>> depositValidatorMock = new();
    private readonly Mock<IValidator<BuyRequestDto>> buyValidatorMock = new();
    private readonly Mock<IJwtService> jwtServiceMock = new();
    private readonly Mock<IUserRepository> userRepoMock = new();
    private readonly Mock<IProductRepository> productRepoMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();

    private readonly FlapKap.Application.Services.Implementations.TransactionService service;

    public TransactionService_ResetWithdrawAsync_Tests()
    {
        service = new FlapKap.Application.Services.Implementations.TransactionService(
            depositValidatorMock.Object,
            buyValidatorMock.Object,
            jwtServiceMock.Object,
            userRepoMock.Object,
            productRepoMock.Object,
            unitOfWorkMock.Object);
    }
    [Fact]
    public async Task ResetWithdrawAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        jwtServiceMock.Setup(j => j.GetUserId()).Returns(1);
        userRepoMock.Setup(r => r.GetUserById(1)).ReturnsAsync((ApplicationUser)null!);

        var result = await service.ResetWithdrawAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("User not found.", result.Errors);
    }

    [Fact]
    public async Task ResetWithdrawAsync_ShouldReturnFailure_WhenBalanceLessThanFive()
    {
        var user = new ApplicationUser { Balance = 2 };
        jwtServiceMock.Setup(j => j.GetUserId()).Returns(1);
        userRepoMock.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var result = await service.ResetWithdrawAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains($"Balance Should Be Greater Than {(int)CoinsEnum.Five}!", result.Errors);
    }

    [Fact]
    public async Task ResetWithdrawAsync_ShouldBreakdownCoinsAndResetBalance_WhenBalanceValid()
    {
        var balance = 185;
        var user = new ApplicationUser { Balance = balance };
        jwtServiceMock.Setup(j => j.GetUserId()).Returns(1);
        userRepoMock.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var result = await service.ResetWithdrawAsync();

        var typed = Assert.IsType<Result<ResetResponseDto>>(result);
        Assert.True(typed.IsSuccess);

        var response = typed.Data;
        Assert.Equal(0, response.RemainingBalance);
        Assert.Equal(balance, response.AmountInCoins.Total);
        Assert.Equal(0, user.Balance);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task ResetWithdrawAsync_ShouldBreakdownCoinsAndResetBalanceWithRemaining_WhenBalanceValid()
    {
        var balance = 187;
        var expectedWithdraw = balance - (balance % (int)CoinsEnum.Five);
        var expectedRemaining = balance - expectedWithdraw;
        var user = new ApplicationUser { Balance = balance };
        jwtServiceMock.Setup(j => j.GetUserId()).Returns(1);
        userRepoMock.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var result = await service.ResetWithdrawAsync();

        var typed = Assert.IsType<Result<ResetResponseDto>>(result);
        Assert.True(typed.IsSuccess);

        var response = typed.Data;
        Assert.Equal(expectedRemaining, response.RemainingBalance);
        Assert.Equal(expectedWithdraw, response.AmountInCoins.Total);
        Assert.Equal(expectedRemaining, user.Balance);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}