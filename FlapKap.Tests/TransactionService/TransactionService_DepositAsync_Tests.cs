using FlapKap.Application.DTOs.Transaction;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Repositories;
using FlapKap.Application.Validators.Transaction;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using Moq;

namespace FlapKap.Tests.TransactionService;

public class TransactionService_DepositAsync_Tests
{
    private readonly Mock<IValidator<DepositCoinsDto>> depositValidator = new();
    private readonly Mock<IJwtService> jwtServiceMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<IProductRepository> productRepositoryMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();

    private readonly FlapKap.Application.Services.Implementations.TransactionService service;

    public TransactionService_DepositAsync_Tests()
    {
        service = new FlapKap.Application.Services.Implementations.TransactionService(
            depositValidator: depositValidator.Object,
            buyValidator: null,
            jwtService: jwtServiceMock.Object,
            userRepository: userRepositoryMock.Object,
            productRepository: productRepositoryMock.Object,
            unitOfWork: unitOfWorkMock.Object
        );

        depositValidator.Setup(v => v.ValidateAsync(It.IsAny<DepositCoinsDto>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    }
    [Fact]
    public async Task DepositAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var deposit = new DepositCoinsDto { FiftyCent = 1 , TwentyCent = 2, TenCent = 1 }; //total 100 cents
        var userId = 1;

        jwtServiceMock.Setup(x => x.GetUserId()).Returns(userId);
        userRepositoryMock.Setup(x => x.GetUserById(userId)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await service.DepositAsync(deposit);

        // Assert
        Assert.False(result.IsSuccess);
        var errorResult = Assert.IsType<Result<string>>(result);
        Assert.Contains("Only Users can deposit.", errorResult.Errors);
    }

    [Fact]
    public async Task DepositAsync_UserFound_UpdatesBalanceAndSavesChanges()
    {
        // Arrange
        var deposit = new DepositCoinsDto { TwentyCent = 2, TenCent = 1 , HundredCent = 1 }; //total 150
        var userId = 1;
        var user = new ApplicationUser { Id = userId, Balance = 50 };

        jwtServiceMock.Setup(x => x.GetUserId()).Returns(userId);
        userRepositoryMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

        // Act
        var result = await service.DepositAsync(deposit);

        // Assert
        Assert.True(result.IsSuccess);
        var typedResult = Assert.IsType<Result<string>>(result);
        Assert.Equal(200, user.Balance); // 50 + 150

        var expectedMsg = $"Deposit successful of Total Amount:{deposit.Total} and Total Balance:{user.Balance}.";
        Assert.Equal(expectedMsg, typedResult.Data);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}