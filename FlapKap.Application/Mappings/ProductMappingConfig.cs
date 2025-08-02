using FlapKap.Application.DTOs.Product;
using FlapKap.Domain.Entities;
using Mapster;

namespace FlapKap.Application.Mappings;


public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>();

        config.NewConfig<CreateProductDto, Product>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.SellerId);

        config.NewConfig<UpdateProductDto, Product>()
            .Ignore(dest => dest.SellerId);
    }

}