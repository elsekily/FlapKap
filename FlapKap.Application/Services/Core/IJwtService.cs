using FlapKap.Application.DTOs.Auth;
using FlapKap.Domain.Common;
using FlapKap.Domain.Entities;
using System.Security.Claims;

namespace FlapKap.Application.Services.Core;

public interface IJwtService
{
    Task<Result<TokenResponseDto>> GenerateTokenAsync(ApplicationUser user);
    void BlacklistToken();
    bool IsTokenBlacklisted();
    int GetUserId();
}