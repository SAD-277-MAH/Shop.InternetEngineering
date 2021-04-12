using Microsoft.EntityFrameworkCore;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using Shop.Repo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repo.Repositories.Repo
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly DbContext _db;

        public CouponRepository(DbContext db) : base(db)
        {
            _db = (_db ?? (DatabaseContext)_db);
        }

        public bool CodeExists(string code)
        {
            if (Get(c => c.Code == code, string.Empty) != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            if (await GetAsync(c => c.Code == code, string.Empty) != null)
            {
                return true;
            }

            return false;
        }
    }
}
