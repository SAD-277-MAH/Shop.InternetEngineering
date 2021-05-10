using Microsoft.AspNetCore.Identity;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using Shop.Services.Seed.Interface;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shop.Services.Seed.Service
{
    public class SeedService : ISeedService
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public SeedService(IUnitOfWork<DatabaseContext> db, UserManager<User> userManager, RoleManager<Role> roleManager, IUserService userService)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedRoles()
        {
            if (!_roleManager.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role() {Name = "Admin"},
                    new Role() {Name = "User"}
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
            }
        }

        public void SeedSetting()
        {
            if (!_db.SettingRepository.Get().Any())
            {
                var setting = new Setting();
                _db.SettingRepository.Add(setting);
                _db.Save();
            }
        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any(u=>u.NormalizedUserName == "ADMIN@ADMIN"))
            {
                // Admin
                var adminUser = new User()
                {
                    UserName = "admin@admin",
                    Email = "admin@admin",
                    FullName = "مدیر وبسایت",
                    PhotoUrl = "/images/site/default.png",
                    IsActive = true,
                    EmailConfirmed = true,
                    RegisterDate = DateTime.Now
                };
                var adminResult = _userManager.CreateAsync(adminUser, "1234").Result;
                if (adminResult.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("admin@admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin" }).Wait();
                }
            }
        }
    }
}
