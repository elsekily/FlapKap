namespace FlapKap.Application.DTOs.Transaction;

public class ResetResponseDto
{
    public DepositCoinsDto AmountInCoins { get; set; }
    public decimal RemainingBalance { get; set; }
}
