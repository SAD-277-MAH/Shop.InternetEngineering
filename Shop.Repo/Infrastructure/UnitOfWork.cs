using Microsoft.EntityFrameworkCore;
using Shop.Repo.Repositories.Interface;
using Shop.Repo.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repo.Infrastructure
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, new()
    {
        #region Constructor
        protected readonly DbContext _db;

        public UnitOfWork()
        {
            _db = new TContext();
        }
        #endregion

        #region Repository
        private IUserRepository userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(_db);
                }
                return userRepository;
            }
        }

        private IRoleRepository roleRepository;
        public IRoleRepository RoleRepository
        {
            get
            {
                if (roleRepository == null)
                {
                    roleRepository = new RoleRepository(_db);
                }
                return roleRepository;
            }
        }

        private ISettingRepository settingRepository;
        public ISettingRepository SettingRepository
        {
            get
            {
                if (settingRepository == null)
                {
                    settingRepository = new SettingRepository(_db);
                }
                return settingRepository;
            }
        }

        private ICategoryRepository categoryRepository;
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (categoryRepository == null)
                {
                    categoryRepository = new CategoryRepository(_db);
                }
                return categoryRepository;
            }
        }

        private IProductRepository productRepository;
        public IProductRepository ProductRepository
        {
            get
            {
                if (productRepository == null)
                {
                    productRepository = new ProductRepository(_db);
                }
                return productRepository;
            }
        }

        private IProductImageRepository productImageRepository;
        public IProductImageRepository ProductImageRepository
        {
            get
            {
                if (productImageRepository == null)
                {
                    productImageRepository = new ProductImageRepository(_db);
                }
                return productImageRepository;
            }
        }
        #endregion

        #region Save
        public bool Save()
        {
            if (_db.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<bool> SaveAsync()
        {
            if (await _db.SaveChangesAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Destructor
        ~UnitOfWork()
        {
            Dispose(false);
        }
        #endregion
    }
}
