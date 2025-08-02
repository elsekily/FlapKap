namespace FlapKap.Application.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = default!;
    public int AmountAvailable { get; set; }
    public decimal Cost { get; set; }
}
