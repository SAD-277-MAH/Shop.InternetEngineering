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
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly DbContext _db;

        public UserRepository(DbContext db) : base(db)
        {
            _db = (_db ?? (DatabaseContext)_db);
        }

        public bool UserExists(string username)
        {
            var temp = username.ToUpper();
            if (Get(u => u.NormalizedUserName == temp, string.Empty) != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var temp = username.ToUpper();
            if (await GetAsync(u => u.NormalizedUserName == temp, string.Empty) != null)
            {
                return true;
            }

            return false;
        }
    }
}
