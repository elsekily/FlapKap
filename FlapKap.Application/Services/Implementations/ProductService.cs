using FlapKap.Application.DTOs.Product;
using FlapKap.Application.DTOs.User;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FlapKap.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IValidator<CreateProductDto> createProductValidator;
    private readonly IValidator<UpdateProductDto> updateProductValidator;
    private readonly IJwtService jwtService;

    public ProductService(IProductRepository productRepository,
                          IUnitOfWork unitOfWork,
                          IValidator<CreateProductDto> createProductValidator,
                          IValidator<UpdateProductDto> updateProductValidator,
                          IJwtService jwtService)
    {
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
        this.createProductValidator = createProductValidator;
        this.updateProductValidator = updateProductValidator;
        this.jwtService = jwtService;
    }
    public async Task<Result<List<ProductDto>>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();
        return Result<List<ProductDto>>.Success(products.Adapt<List<ProductDto>>());
    }

    public async Task<Result<ProductDto>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        return Result<ProductDto>.Success(product.Adapt<ProductDto>());
    }

    public async Task<Result> CreateAsync(CreateProductDto dto)
    {
        var validation = await createProductValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<ValidationResult>.Failure("Please validate data!",validation);

        var product = dto.Adapt<Product>();
        product.SellerId = jwtService.GetUserId();

        var dbProduct = await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        return Result<ProductDto>.Success(dbProduct.Adapt<ProductDto>());
    }
    public async Task<Result> UpdateAsync(UpdateProductDto updated)
    {
        var validation = await updateProductValidator.ValidateAsync(updated);
        if (!validation.IsValid)
            return Result<ValidationResult>.Failure("Please validate data!", validation);


        var existing = await productRepository.GetByIdAsync(updated.Id);
        if (existing == null)
            return Result.Failure("Product not found.");

        var userId = jwtService.GetUserId();
        if (existing.SellerId != userId)
            return Result.Failure("Access denied. You are not the owner.");

        updated.SellerId = userId;


        updated.Adapt(existing);
        await unitOfWork.SaveChangesAsync();

        return Result<ProductDto>.Success(existing.Adapt<ProductDto>());
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
            return Result.Failure("Product not found.");

        if (product.SellerId != jwtService.GetUserId())
            return Result.Failure("Access denied. You are not the owner.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}