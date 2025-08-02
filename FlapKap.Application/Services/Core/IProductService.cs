using FlapKap.Application.DTOs.Product;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;

namespace FlapKap.Application.Services.Core;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllAsync();
    Task<Result<ProductDto>> GetByIdAsync(int id);
    Task<Result> CreateAsync(CreateProductDto product);
    Task<Result> UpdateAsync(UpdateProductDto product);
    Task<Result> DeleteAsync(int id);
}
