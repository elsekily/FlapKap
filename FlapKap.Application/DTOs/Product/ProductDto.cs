using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Application.DTOs.Product;
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int AmountAvailable { get; set; }
    public decimal Cost { get; set; }
}