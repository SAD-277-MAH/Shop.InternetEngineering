using Microsoft.AspNetCore.Identity;
using Shop.Data.Models;
using Shop.Data.ViewModels.Panel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Scopes
{
    public class PanelLayoutScope
    {
        private readonly UserManager<User> _userManager;

        public PanelLayoutScope(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDetailViewModel> GetUserDetail(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            PersianCalendar pc = new PersianCalendar();

            var userDetail = new UserDetailViewModel()
            {
                FullName = user.FullName,
                PhotoUrl = user.PhotoUrl,
                RegisterDate = $"{pc.GetYear(user.RegisterDate)}/{pc.GetMonth(user.RegisterDate)}/{pc.GetDayOfMonth(user.RegisterDate)}"
            };

            return userDetail;
        }
    }
}
