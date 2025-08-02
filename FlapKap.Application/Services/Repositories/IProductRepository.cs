using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Application.Services.Repositories;
public interface IProductRepository : IGenericRepository<Product>
{
    public Task<List<Product>> GetProductsByIds(List<int> id);
}