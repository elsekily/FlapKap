using FlapKap.Application.DTOs.Transaction;
using FlapKap.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Application.Services.Core;

public interface ITransactionService
{
    Task<Result> DepositAsync(DepositCoinsDto deposit);
    Task<Result> BuyAsync(BuyRequestDto request);
    Task<Result> ResetWithdrawAsync();
}
