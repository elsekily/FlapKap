using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Entities;
using FlapKap.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FlapKap.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext dbContext;

    public UserRepository(ApplicationDbContext context)
    {
        this.dbContext = context;
    }

    public async Task<ApplicationUser> GetUserById(int userId)
    {
        return await dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
    }
}