using FlapKap.Application.DTOs.Transaction;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Implementations;
using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using Moq;

namespace FlapKap.Tests.TransactionService;

public class TransactionService_BuyAsync_Tests
{
    private readonly Mock<IValidator<BuyRequestDto>> buyValidatorMock = new();
    private readonly Mock<IJwtService> jwtServiceMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<IProductRepository> productRepositoryMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();

    private readonly FlapKap.Application.Services.Implementations.TransactionService service;

    public TransactionService_BuyAsync_Tests()
    {
        service = new FlapKap.Application.Services.Implementations.TransactionService (
            depositValidator: null, // not used
            buyValidator: buyValidatorMock.Object,
            jwtService: jwtServiceMock.Object,
            userRepository: userRepositoryMock.Object,
            productRepository: productRepositoryMock.Object,
            unitOfWork: unitOfWorkMock.Object
        );

        buyValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<BuyRequestDto>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnFailure_WhenInvalidProductIds()
    {
        var dto = new BuyRequestDto
        {
            Products = [new() { ProductId = 1, Quantity = 1 }]
        };

        productRepositoryMock.Setup(r => r.GetProductsByIds(It.IsAny<List<int>>()))
            .ReturnsAsync([]);

        var result = await service.BuyAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains("Please Enter Valid ProductIds", result.Errors);
        unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        var dto = new BuyRequestDto { Products = [new() { ProductId = 1, Quantity = 1 }] };

        productRepositoryMock.Setup(r => r.GetProductsByIds(It.IsAny<List<int>>()))
            .ReturnsAsync([new Product { Id = 1, AmountAvailable = 10, Cost = 50 }]);

        jwtServiceMock.Setup(s => s.GetUserId()).Returns(1);
        userRepositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync((ApplicationUser)null);

        var result = await service.BuyAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains("Only users can perform this action.", result.Errors);
        unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnFailure_WhenStockIsInsufficient()
    {
        var dto = new BuyRequestDto { Products = [new() { ProductId = 1, Quantity = 5 }] };

        productRepositoryMock.Setup(r => r.GetProductsByIds(It.IsAny<List<int>>()))
            .ReturnsAsync([new Product { Id = 1, AmountAvailable = 2, Cost = 10 }]);

        jwtServiceMock.Setup(s => s.GetUserId()).Returns(1);
        userRepositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync(new ApplicationUser { Id = 1, Balance = 100 });

        var result = await service.BuyAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains("Product 1 has only 2 items left.", result.Errors);
        unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnFailure_WhenInsufficientBalance()
    {
        var dto = new BuyRequestDto { Products = [new() { ProductId = 1, Quantity = 2 }] };

        productRepositoryMock.Setup(r => r.GetProductsByIds(It.IsAny<List<int>>()))
            .ReturnsAsync([new Product { Id = 1, AmountAvailable = 10, Cost = 80 }]);

        jwtServiceMock.Setup(s => s.GetUserId()).Returns(1);
        userRepositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync(new ApplicationUser { Id = 1, Balance = 100 });

        var result = await service.BuyAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains("Insufficient balance.", result.Errors);
        unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnSuccess_WhenEverythingValid()
    {
        var dto = new BuyRequestDto { Products = [new() { ProductId = 1, Quantity = 2 }] };
        var product = new Product { Id = 1, AmountAvailable = 5, Cost = 30 };
        var user = new ApplicationUser { Id = 1, Balance = 100 };

        productRepositoryMock.Setup(r => r.GetProductsByIds(It.IsAny<List<int>>()))
            .ReturnsAsync([product]);
        jwtServiceMock.Setup(s => s.GetUserId()).Returns(1);
        userRepositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var result = await service.BuyAsync(dto);

        var typed = Assert.IsType<Result<BuyResponseDto>>(result);
        Assert.True(typed.IsSuccess);
        Assert.Equal(40, typed.Data.RemainingBalance);
        Assert.Equal(60, typed.Data.TotalSpent);

        unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}