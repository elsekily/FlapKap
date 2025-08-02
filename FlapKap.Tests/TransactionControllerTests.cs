using FlapKap.API.Controllers;
using FlapKap.Application.DTOs.Transaction;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;

namespace FlapKap.Tests;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _transactionServiceMock = new Mock<ITransactionService>();
        _controller = new TransactionController(_transactionServiceMock.Object);
    }

    [Fact]
    public async Task Deposit_ReturnsOk_WhenSuccess()
    {
        var dto = new DepositCoinsDto();
        _transactionServiceMock.Setup(s => s.DepositAsync(dto))
            .ReturnsAsync(Result<string>.Success("Deposit successful"));

        var result = await _controller.Deposit(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Deposit successful", ((Result<string>)okResult.Value).Data);
    }

    [Fact]
    public async Task Deposit_ReturnsBadRequest_WhenFailure()
    {
        var dto = new DepositCoinsDto();
        _transactionServiceMock.Setup(s => s.DepositAsync(dto))
            .ReturnsAsync(Result.Failure("Invalid deposit"));

        var result = await _controller.Deposit(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Buy_ReturnsOk_WhenSuccess()
    {
        var dto = new BuyRequestDto
        {
            Products = new List<BuyProductDto>()
        };

        _transactionServiceMock.Setup(s => s.BuyAsync(dto))
            .ReturnsAsync(Result<BuyResponseDto>.Success(new BuyResponseDto()));

        var result = await _controller.Buy(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Buy_ReturnsBadRequest_WhenFailure()
    {
        var dto = new BuyRequestDto
        {
            Products = new List<BuyProductDto>()
        };

        _transactionServiceMock.Setup(s => s.BuyAsync(dto))
            .ReturnsAsync(Result.Failure("Invalid"));

        var result = await _controller.Buy(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Reset_ReturnsOk_WhenSuccess()
    {
        var dto = new ResetResponseDto();
        _transactionServiceMock.Setup(s => s.ResetWithdrawAsync())
            .ReturnsAsync(Result<ResetResponseDto>.Success(dto));

        var result = await _controller.Reset();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Reset_ReturnsBadRequest_WhenFailure()
    {
        _transactionServiceMock.Setup(s => s.ResetWithdrawAsync())
            .ReturnsAsync(Result.Failure("Invalid reset"));

        var result = await _controller.Reset();

        Assert.IsType<BadRequestObjectResult>(result);
    }
}