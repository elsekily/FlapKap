namespace FlapKap.Application.DTOs.Transaction;

public class BuyRequestDto
{
    public List<BuyProductDto> Products { get; set; } = new();
}