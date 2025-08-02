using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlapKap.Persistence.Data;

public static class AutomatedMigration
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            //var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await DbSeeder.SeedAsync(context, userManager, roleManager);
            //await unitOfWork.SaveChangesAsync();
        }
    }
}