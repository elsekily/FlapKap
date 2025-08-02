using FlapKap.Application.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlapKap.Domain.Constants;
using FlapKap.Application.DTOs.Transaction;

namespace FlapKap.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.Buyer)]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        this.transactionService = transactionService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositCoinsDto deposit)
    {
        var result = await transactionService.DepositAsync(deposit);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("buy")]
    public async Task<IActionResult> Buy([FromBody] BuyRequestDto request)
    {
        var result = await transactionService.BuyAsync(request);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var result = await transactionService.ResetWithdrawAsync();
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}