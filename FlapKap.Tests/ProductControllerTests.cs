using FlapKap.API.Controllers;
using FlapKap.Application.DTOs.Product;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlapKap.Tests;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Result<List<ProductDto>>.Success(new List<ProductDto>()));

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenFailure()
    {
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(Result<List<ProductDto>>.Failure("Failed"));

        var result = await _controller.GetAll();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Result<ProductDto>.Success(new ProductDto()));

        var result = await _controller.GetById(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Result<ProductDto>.Failure("Not found"));

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateProductDto>()))
            .ReturnsAsync(Result<ProductDto>.Success(new ProductDto()));

        var result = await _controller.Create(new CreateProductDto());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenFailure()
    {
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateProductDto>()))
            .ReturnsAsync(Result.Failure("Bad data"));

        var result = await _controller.Create(new CreateProductDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<UpdateProductDto>()))
            .ReturnsAsync(Result<ProductDto>.Success(new ProductDto()));

        var result = await _controller.Update(new UpdateProductDto());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenFailure()
    {
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<UpdateProductDto>()))
            .ReturnsAsync(Result.Failure("Failed update"));

        var result = await _controller.Update(new UpdateProductDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccess()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Success());

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenFailure()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Failure("Not found"));

        var result = await _controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}