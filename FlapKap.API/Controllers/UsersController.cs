using FlapKap.Application.Services.Core;
using FlapKap.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlapKap.Domain.Common;
using FlapKap.Application.DTOs.User;
using FlapKap.Domain.Constants;

namespace FlapKap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet("data")]
    [Authorize(Policy = AuthConstants.MatchUsername)]
    public async Task<IActionResult> GetUserByUserName([FromQuery] string username)
    {
        var result = await userService.GetUserByUsernameAsync(username);
        
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost(RoleConstants.Buyer)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateBuyer(CreateUserDto createUserDto)
    {
        var result = await userService.CreateUserAsync(createUserDto, RoleConstants.Buyer);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost(RoleConstants.Seller)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateSeller(CreateUserDto createUserDto)
    {
        var result = await userService.CreateUserAsync(createUserDto,RoleConstants.Seller);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Policy = AuthConstants.MatchUsername)]
    public async Task<IActionResult> UpdateUser([FromQuery] string username, UpdateUserDto updateUserDto)
    {
        var result = await userService.UpdateUserAsync(username, updateUserDto);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AuthConstants.MatchUsername)]
    public async Task<IActionResult> DeleteUser([FromQuery] string username)
    {
        var result = await userService.DeleteUserAsync(username);
        
        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
} 