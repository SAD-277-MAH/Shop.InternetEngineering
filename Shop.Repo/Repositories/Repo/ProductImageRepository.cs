using Microsoft.EntityFrameworkCore;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using Shop.Repo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Repo.Repositories.Repo
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly DbContext _db;

        public ProductImageRepository(DbContext db) : base(db)
        {
            _db = (_db ?? (DatabaseContext)_db);
        }
    }
}
