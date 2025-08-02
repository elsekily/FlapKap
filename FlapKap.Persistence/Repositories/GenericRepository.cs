using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FlapKap.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlapKap.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext dbContext;

    public GenericRepository(ApplicationDbContext context)
    {
        this.dbContext = context;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IList<T>> GetAllAsync()
    {
        return await dbContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbContext.Set<T>().AnyAsync(predicate);
    }
} 