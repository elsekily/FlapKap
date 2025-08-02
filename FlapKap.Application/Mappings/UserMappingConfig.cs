using FlapKap.Application.DTOs.Product;
using FlapKap.Application.DTOs.User;
using FlapKap.Domain.Entities;
using Mapster;

namespace FlapKap.Application.Mappings;


public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ApplicationUser, UserResponseDto>()
            .Map(dest => dest.Roles, src => new List<string>());


        config.NewConfig<CreateUserDto, ApplicationUser>()
            .Map(dest => dest.Id, src => 0)
            .Map(dest => dest.Email, src => $"{src.Username}@flapkap.com")
            .Map(dest => dest.UserName, src => src.Username)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);

        config.NewConfig<UpdateUserDto, ApplicationUser>()
            .Map(dest => dest.Email, src => $"{src.Username}@flapkap.com")
            .Map(dest => dest.UserName, src => src.Username)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);
    }
}