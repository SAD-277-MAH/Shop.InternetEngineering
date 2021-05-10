using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Account;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class UsersManagementController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UsersManagementController(IUnitOfWork<DatabaseContext> db, IMapper mapper, UserManager<User> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var users = await _db.UserRepository.GetAsync();

            var usersDetails = _mapper.Map<List<UserDetailsViewModel>>(users);
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    usersDetails.Where(u => u.UserName.ToUpper() == user.NormalizedUserName).Single().RoleName = "مدیر سیستم";
                }
                else
                {
                    usersDetails.Where(u => u.UserName.ToUpper() == user.NormalizedUserName).Single().RoleName = "کاربر";
                }
            }

            return View(usersDetails.OrderBy(u => u.RoleName).ThenByDescending(u => u.UserName));
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _db.UserRepository.GetAsync(u => u.Id == id, string.Empty);
            if (user != null)
            {
                var userDetails = _mapper.Map<UserFullDetailsViewModel>(user);

                PersianCalendar pc = new PersianCalendar();
                userDetails.RegisterDate = $"{pc.GetYear(user.RegisterDate)}/{pc.GetMonth(user.RegisterDate)}/{pc.GetDayOfMonth(user.RegisterDate)}";

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    userDetails.RoleName = "مدیر سیستم";
                }
                else
                {
                    userDetails.RoleName = "کاربر";
                }

                return View(userDetails);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region ChangeRole
        public IActionResult ChangeRole([FromQuery] string UserName, [FromQuery] string RoleName)
        {
            ViewData["RoleName"] = RoleName;
            ViewData["UserName"] = UserName;
            return View();
        }

        [HttpPost, ActionName("ChangeRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRoleConfirm([FromQuery] string UserName, [FromQuery] string RoleName)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (await _userManager.IsInRoleAsync(user, RoleName))
                {
                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
                else
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, RoleName);

                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
            }
            else
            {
                ViewData["RoleName"] = RoleName;
                ViewData["UserName"] = UserName;
                return View();
            }
        }
        #endregion

        #region ChangeActive
        public IActionResult ChangeActive([FromQuery] string UserName, [FromQuery] bool Status)
        {
            ViewData["Status"] = Status;
            ViewData["UserName"] = UserName;
            return View();
        }

        [HttpPost, ActionName("ChangeActive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeActiveConfirm([FromQuery] string UserName, [FromQuery] bool Status)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (user.IsActive == Status)
                {
                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
                else
                {
                    user.IsActive = Status;
                    _db.UserRepository.Update(user);
                    await _db.SaveAsync();

                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
            }
            else
            {
                ViewData["Status"] = Status;
                ViewData["UserName"] = UserName;
                return View();
            }
        }
        #endregion

        #region ConfirmEmail
        public IActionResult ConfirmEmail([FromQuery] string UserName)
        {
            ViewData["UserName"] = UserName;
            return View();
        }

        [HttpPost, ActionName("ConfirmEmail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmailConfirm([FromQuery] string UserName)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (user.EmailConfirmed)
                {
                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
                else
                {
                    user.EmailConfirmed = true;
                    _db.UserRepository.Update(user);
                    await _db.SaveAsync();

                    return Redirect("/Admin/UsersManagement/Details/" + user.Id);
                }
            }
            else
            {
                ViewData["UserName"] = UserName;
                return View();
            }
        }
        #endregion

        #region ChangePassword
        public IActionResult ChangePassword([FromQuery] string UserName)
        {
            ViewData["UserName"] = UserName;
            return View();
        }

        [HttpPost, ActionName("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordConfirm([FromQuery] string UserName, SetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, viewModel.Password);

                return Redirect("/Admin/UsersManagement/Details/" + user.Id);
            }
            else
            {
                ViewData["UserName"] = UserName;
                return View();
            }
        }
        #endregion
    }
}
