using FlapKap.Application.Services.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace FlapKap.Application.Services.Implementations;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache cache;
    private readonly IHttpContextAccessor httpContextAccessor;
    private const string CacheKeyPrefix = "BlacklistedToken_";

    public TokenBlacklistService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
    {
        this.cache = cache;
        this.httpContextAccessor = httpContextAccessor;
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
}