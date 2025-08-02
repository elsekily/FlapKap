using FlapKap.Domain.Constants;
using FlapKap.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FlapKap.Persistence.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[] { RoleConstants.Buyer, RoleConstants.Seller };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }
    }
    public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserWithRole(userManager, "testbuyer@test.com", RoleConstants.Buyer);
        await CreateUserWithRole(userManager, "testseller@test.com", RoleConstants.Seller);
    }

    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);

        // Save changes
        await context.SaveChangesAsync();
    }

    private static async Task CreateUserWithRole(UserManager<ApplicationUser> userManager, string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, "Test123!");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
