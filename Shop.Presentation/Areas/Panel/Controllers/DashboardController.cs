using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Extentions;
using Shop.Common.Helpers.Interface;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Common;
using Shop.Data.ViewModels.Panel;
using Shop.Repo.Infrastructure;
using Shop.Services.Upload.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Policy = "RequireUserRole")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly UserManager<User> _userManager;
        private readonly IUploadService _uploadService;
        private readonly IUtilities _utilities;

        public DashboardController(IUnitOfWork<DatabaseContext> db, UserManager<User> userManager, IUploadService uploadService, IUtilities utilities)
        {
            _db = db;
            _userManager = userManager;
            _uploadService = uploadService;
            _utilities = utilities;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            PersianCalendar pc = new PersianCalendar();

            var userDetail = new UserDetailDashboardViewModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                RegisterDate = $"{pc.GetYear(user.RegisterDate)}/{pc.GetMonth(user.RegisterDate)}/{pc.GetDayOfMonth(user.RegisterDate)}"
            };

            return View(userDetail);
        }
        #endregion

        #region EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userForm = new ProfileEditViewModel()
            {
                Email = user.Email,
                FullName = user.FullName
            };

            return View(userForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileEditViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            viewModel.Email = user.Email;
            if (ModelState.IsValid)
            {
                string url = user.PhotoUrl;
                if (viewModel.File.IsImage())
                {
                    var uploadResult = await _uploadService.UploadFile(viewModel.File, string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value), "Users\\" + user.Id + "\\Profile");

                    if (uploadResult.Status)
                    {
                        if (url != "/images/site/default.png")
                        {
                            try
                            {
                                _uploadService.RemoveFileFromLocal(_utilities.FindLocalPathFromUrl(url));
                            }
                            catch { }
                        }

                        url = uploadResult.Url;
                    }
                    else
                    {
                        ModelState.AddModelError("File", "خطا در آپلود تصویر پروفایل");
                        return View(viewModel);
                    }
                }

                user.FullName = viewModel.FullName;
                user.PhotoUrl = url;
                _db.UserRepository.Update(user);
                await _db.SaveAsync();

                return Redirect("/Panel");
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion

        #region ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var result = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.Password);
                if (result.Succeeded)
                {
                    return Redirect("/Panel");
                }
                else
                {
                    ModelState.AddModelError("", "خطا در تغییر کلمه عبور");
                    return View(viewModel);
                }
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion

        #region UploadProfilePhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                string url = user.PhotoUrl;
                if (file.IsImage())
                {
                    var uploadResult = await _uploadService.UploadFile(file, string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value), "Users\\" + user.Id + "\\Profile");

                    if (uploadResult.Status)
                    {
                        if (url != "/images/site/default.png")
                        {
                            try
                            {
                                _uploadService.RemoveFileFromLocal(_utilities.FindLocalPathFromUrl(url));
                            }
                            catch { }
                        }

                        url = uploadResult.Url;
                    }
                    else
                    {
                        return Redirect("/Panel");
                    }
                }

                user.PhotoUrl = url;
                _db.UserRepository.Update(user);
                await _db.SaveAsync();

                return Redirect("/Panel");
            }
            else
            {
                return Redirect("/Panel");
            }
        }
        #endregion
    }
}
