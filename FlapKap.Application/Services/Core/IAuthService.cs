using FlapKap.Application.DTOs.Auth;
using FlapKap.Domain.Common;
using System.Net.Http;

namespace FlapKap.Application.Services.Core;

public interface IAuthService
{
    Task<Result> LoginAsync(LoginDto loginDto);
}