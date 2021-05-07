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
        IProductImageRepository ProductImageRepository { get; }
        ISocialRepository SocialRepository { get; }
        ILicenseRepository LicenseRepository { get; }
        ICouponRepository CouponRepository { get; }
        ICouponUserRepository CouponUserRepository { get; }
        ICouponProductRepository CouponProductRepository { get; }
        ICouponCategoryRepository CouponCategoryRepository { get; }
        IAddressRepository AddressRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        ICouponOrderRepository CouponOrderRepository { get; }

        bool Save();
        Task<bool> SaveAsync();
    }
}
