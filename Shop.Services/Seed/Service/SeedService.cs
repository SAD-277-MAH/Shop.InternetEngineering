using Microsoft.AspNetCore.Identity;
using Shop.Data.Models;
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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserService _userService;

        public SeedService(UserManager<User> userManager, RoleManager<Role> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
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
