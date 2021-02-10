using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repo.Repositories.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        bool UserExists(string username);
        Task<bool> UserExistsAsync(string username);
    }
}
