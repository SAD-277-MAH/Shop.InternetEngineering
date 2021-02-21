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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly DbContext _db;

        public ProductRepository(DbContext db) : base(db)
        {
            _db = (_db ?? (DatabaseContext)_db);
        }
    }
}
