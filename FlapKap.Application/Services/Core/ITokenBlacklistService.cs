namespace FlapKap.Application.Services.Core;

public interface ITokenBlacklistService
{
    void BlacklistToken();
    bool IsTokenBlacklisted();
}