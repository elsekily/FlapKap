namespace FlapKap.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public int AmountAvailable { get; set; }
    public decimal Cost { get; set; }
    public int SellerId { get; set; }
    public ApplicationUser Seller { get; set; }
}