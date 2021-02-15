using Microsoft.AspNetCore.Identity;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Scopes
{
    public class AdminLayoutScope
    {
        private readonly UserManager<User> _userManager;

        public AdminLayoutScope(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDetailViewModel> GetUserDetail(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userDetail = new UserDetailViewModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                RoleName = "مدیر وبسایت"
            };

            return userDetail;
        }
    }
}
