using FlapKap.Application.DTOs.Product;
using FlapKap.Application.Services.Core;
using FlapKap.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlapKap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await productService.GetAllAsync();

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Seller)]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var result = await productService.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = RoleConstants.Seller)]
    public async Task<IActionResult> Update(UpdateProductDto dto)
    {
        var result = await productService.UpdateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.Seller)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productService.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
}