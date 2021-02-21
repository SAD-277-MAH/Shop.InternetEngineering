using Microsoft.EntityFrameworkCore;
using Shop.Repo.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repo.Infrastructure
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISettingRepository SettingRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }

        bool Save();
        Task<bool> SaveAsync();
    }
}
