using FlapKap.Domain.Entities;

namespace FlapKap.Application.Services.Repositories;

public interface IUserRepository
{
    public Task<ApplicationUser> GetUserById(int userId);
}