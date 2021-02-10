using Microsoft.AspNetCore.Identity;
using Shop.Common.ReturnMessage;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Account;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(IUnitOfWork<DatabaseContext> db, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AccountReturnMessage> Login(LoginViewModel viewModel)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(viewModel.Email);
                return new AccountReturnMessage(true, user, null);
            }
            else if (result.IsLockedOut)
            {
                return new AccountReturnMessage(false, null, new List<string>() { "حساب کاربری شما به مدت ۵ دقیقه قفل شده است" });
            }
            else if (result.IsNotAllowed)
            {
                return new AccountReturnMessage(false, null, new List<string>() { "حساب کاربری فعال نیست. برای فعالسازی حساب کاربری، روی لینک موجود در ایمیل خود کلیک کنید" });
            }
            else
            {
                return new AccountReturnMessage(false, null, new List<string>() { "ایمیل یا کلمه عبور اشتباه است" });
            }
        }

        public async Task<AccountReturnMessage> Register(RegisterViewModel viewModel)
        {
            User user = new User()
            {
                FullName = viewModel.FullName,
                UserName = viewModel.Email,
                Email = viewModel.Email,
                PhotoUrl = "/images/site/default.png",
                IsActive = false,
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                var userAddRole = _userManager.FindByNameAsync(user.UserName).Result;
                await _userManager.AddToRolesAsync(userAddRole, new[] { "User" });
                return new AccountReturnMessage(true, user, null);
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description.ToString());
                }
                return new AccountReturnMessage(false, null, errors);
            }
        }
    }
}
