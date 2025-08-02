using FlapKap.Application.DTOs.Transaction;
using FlapKap.Application.Enums;
using FlapKap.Application.Services.Core;
using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Common;
using FlapKap.Domain.Constants;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace FlapKap.Application.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly IValidator<DepositCoinsDto> depositValidator;
    private readonly IValidator<BuyRequestDto> buyValidator;
    private readonly IJwtService jwtService;
    private readonly IUserRepository userRepository;
    private readonly IProductRepository productRepository;
    private readonly IUnitOfWork unitOfWork;

    public TransactionService(IValidator<DepositCoinsDto> depositValidator,
                              IValidator<BuyRequestDto> buyValidator,
                              IJwtService jwtService,
                              IUserRepository userRepository,
                              IProductRepository productRepository,
                              IUnitOfWork unitOfWork)
    {
        this.depositValidator = depositValidator;
        this.buyValidator = buyValidator;
        this.jwtService = jwtService;
        this.userRepository = userRepository;
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
    }
    public async Task<Result> BuyAsync(BuyRequestDto request)
    {
        var validation = await buyValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<ValidationResult>.Failure("Please validate data!", validation);

        await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
        try
        {
            var products = await productRepository.GetProductsByIds(request.Products.Select(p => p.ProductId).ToList());
            if (products.Count != request.Products.Count)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure("Please Enter Valid ProductIds");
            }

            var user = await userRepository.GetUserById(jwtService.GetUserId());
            if (user == null)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure("Only users can perform this action.");
            }

            var productIdAmountPairs = request.Products.ToDictionary(a => a.ProductId, a => a.Quantity);

            decimal totalCost = 0;
            foreach (var product in products)
            {
                if (product.AmountAvailable < productIdAmountPairs[product.Id])
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return Result.Failure($"Product {product.Id} has only {product.AmountAvailable} items left.");
                }

                totalCost += product.Cost * productIdAmountPairs[product.Id];
                product.AmountAvailable -= productIdAmountPairs[product.Id];
            }

            if (user.Balance < totalCost)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result.Failure("Insufficient balance.");
            }

            // Update user balance
            user.Balance -= totalCost;
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
            return Result<BuyResponseDto>.Success(new BuyResponseDto()
            {
                ItemsQuantityPairs = request.Products,
                RemainingBalance = user.Balance,
                TotalSpent = totalCost
            });
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }

        
    }

    public async Task<Result> DepositAsync(DepositCoinsDto deposit)
    {
        var validation = await depositValidator.ValidateAsync(deposit);
        if (!validation.IsValid)
            return Result<ValidationResult>.Failure("Please validate data!", validation);

        var user = await userRepository.GetUserById(jwtService.GetUserId());
        if (user == null)
            return Result<string>.Failure("Only Users can deposit.");

        user.Balance += deposit.Total;

        await unitOfWork.SaveChangesAsync();

        return Result<string>.Success($"Deposit successful of Total Amount:{deposit.Total} and Total Balance:{user.Balance}.");
    }
    public async Task<Result> ResetWithdrawAsync()
    {
        var user = await userRepository.GetUserById(jwtService.GetUserId());
        if (user == null)
            return Result<string>.Failure("User not found.");

        if(user.Balance < (int)CoinsEnum.Five)
            return Result<string>.Failure($"Balance Should Be Greater Than {(int)CoinsEnum.Five}!");

        var response = GetBalanceInCoinsAndRemaining(user.Balance);
       
        user.Balance = response.RemainingBalance;
        await unitOfWork.SaveChangesAsync();

        return Result<ResetResponseDto>.Success(response);
    }
    private ResetResponseDto GetBalanceInCoinsAndRemaining(decimal amount)
    { 
        var coins = new DepositCoinsDto();

        var denominations = new Dictionary<decimal, Action<int>>
        {
            { (int)CoinsEnum.Hundred, qty => coins.HundredCent = qty },
            { (int)CoinsEnum.Fifty, qty => coins.FiftyCent = qty },
            { (int)CoinsEnum.Twenty, qty => coins.TwentyCent = qty },
            { (int)CoinsEnum.Ten, qty => coins.TenCent = qty },
            { (int)CoinsEnum.Five, qty => coins.FiveCent = qty }
        };

        foreach (var denom in denominations.Keys)
        {
            int count = (int)(amount / denom);
            denominations[denom](count);
            amount %= denom;
        }

        return new ResetResponseDto()
        {
            RemainingBalance = amount,
            AmountInCoins = coins
        };
    }
}