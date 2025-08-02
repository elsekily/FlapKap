using FlapKap.Application.Services.Repositories;
using FlapKap.Domain.Entities;
using FlapKap.Domain.Interfaces;
using FlapKap.Persistence.Data;
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

}
