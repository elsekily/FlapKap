using FlapKap.Application.Services.Core;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FlapKap.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IJwtService jwtService)
    {
        if (jwtService.IsTokenBlacklisted())
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token has been revoked");
            return;
        }

        await _next(context);
    }
}