namespace FlapKap.Application.DTOs.Transaction;

public class BuyResponseDto
{
    public decimal TotalSpent { get; set; }
    public decimal RemainingBalance { get; set; }
    public List<BuyProductDto> ItemsQuantityPairs { get; set; }
}