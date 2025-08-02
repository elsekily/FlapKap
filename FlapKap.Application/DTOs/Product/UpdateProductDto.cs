namespace FlapKap.Application.DTOs.Product;

public class UpdateProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int AmountAvailable { get; set; }
    public decimal Cost { get; set; }
    public int SellerId { get; set; }
}