using FlapKap.Domain.Interfaces;
using FlapKap.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FlapKap.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext dbContext;
    private IDbContextTransaction? transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        this.dbContext = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        transaction = await dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (transaction != null)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    public void Dispose()
    {
        transaction?.Dispose();
        dbContext.Dispose();
    }
} 