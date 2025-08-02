using FlapKap.Application.DTOs.Product;
using FlapKap.Domain.Entities;
using Mapster;

namespace FlapKap.Application.Mappings;

public static class ProductMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Product, ProductDto>.NewConfig();

        TypeAdapterConfig<CreateProductDto, Product>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.SellerId);

        TypeAdapterConfig<UpdateProductDto, Product>
            .NewConfig()
            .Ignore(dest => dest.SellerId);
    }
}