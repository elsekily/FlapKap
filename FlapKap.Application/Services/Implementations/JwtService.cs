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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace FlapKap.Application.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly JwtSettings jwtSettings;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IMemoryCache cache;
    private readonly IHttpContextAccessor httpContextAccessor;
    private const string CacheKeyPrefix = "BlacklistedToken_";

    public JwtService(IOptions<JwtSettings> jwtSettings,
                      UserManager<ApplicationUser> userManager, 
                      IMemoryCache cache, 
                      IHttpContextAccessor httpContextAccessor)
    {
        this.jwtSettings = jwtSettings.Value;
        this.userManager = userManager;
        this.cache = cache;
        this.httpContextAccessor = httpContextAccessor;
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
    public void BlacklistToken()
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return;

        var token = authHeader.Substring("Bearer ".Length);

        var cacheKey = $"{CacheKeyPrefix}{token}";
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };

        cache.Set(cacheKey, true, cacheOptions);
    }
    public bool IsTokenBlacklisted()
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return false;

        var token = authHeader.Substring("Bearer ".Length);
        var cacheKey = $"{CacheKeyPrefix}{token}";


        var x = cache.TryGetValue(cacheKey, out _);
        return x;
    }
    public int GetUserId()
    {
        var id = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(id, out var userId);
        return userId;
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