using FlapKap.Application.DTOs.Auth;
using FlapKap.Application.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace FlapKap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
} 