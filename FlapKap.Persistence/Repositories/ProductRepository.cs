using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FlapKap.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Product>> GetProductsByIds(List<int> ids)
    {
        return await dbContext.Products
                            .Where(p => ids.Contains(p.Id))
                            .ToListAsync(); 
    }
}