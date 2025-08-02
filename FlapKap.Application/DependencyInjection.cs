using FlapKap.Application.Mappings;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Implementations;
using FlapKap.Application.Settings;
using FlapKap.Application.Validators;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlapKap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
      
        // Register Mapster
        services.AddMapster();

        // Register validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

        return services;
    }
} 