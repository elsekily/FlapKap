using FlapKap.Application.DTOs.User;
using FlapKap.Domain.Entities;
using Mapster;

namespace FlapKap.Application.Mappings;

public static class UserMappingConfig
{
    public static void ConfigureUserMappings()
    {
        TypeAdapterConfig<ApplicationUser, UserResponseDto>
            .NewConfig()
            .Map(dest => dest.Roles, src => new List<string>());

        TypeAdapterConfig<CreateUserDto, ApplicationUser>
            .NewConfig()
            .Map(dest => dest.Id, src => 0)
            .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper())
            .Map(dest => dest.UserName, src => src.Email.Split("@",StringSplitOptions.None).FirstOrDefault())
            .Map(dest => dest.NormalizedUserName, src => src.Email.Split("@", StringSplitOptions.None).FirstOrDefault().ToUpper());


        TypeAdapterConfig<UpdateUserDto, ApplicationUser>
            .NewConfig()
            .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper())
            .Map(dest => dest.NormalizedUserName, src => src.Email.Split("@", StringSplitOptions.None).FirstOrDefault().ToUpper());
    }
} 