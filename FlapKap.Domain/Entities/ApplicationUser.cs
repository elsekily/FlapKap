using Microsoft.AspNetCore.Identity;

namespace FlapKap.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public List<Product> Products { get; set; }
} 