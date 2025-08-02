using FlapKap.Application.Settings;
using FlapKap.Domain.Entities;
using FlapKap.Application.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FlapKap.Application.DTOs.Auth;
using FlapKap.Domain.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace FlapKap.Application.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly JwtSettings jwtSettings;
    private readonly UserManager<ApplicationUser> userManager;

    public JwtService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        this.jwtSettings = jwtSettings.Value;
        this.userManager = userManager;
    }

    public async Task<Result<TokenResponseDto>> GenerateTokenAsync(ApplicationUser user)
    {
        var claims = await GetUserClaimsAsync(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );
        
        return Result<TokenResponseDto>.Success(new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        });
    }
    private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}