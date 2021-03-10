using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Extentions;
using Shop.Common.Helpers.Interface;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using Shop.Services.Upload.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUploadService _uploadService;
        private readonly IUtilities _utilities;

        public ProfileController(IUnitOfWork<DatabaseContext> db, IMapper mapper, UserManager<User> userManager, IUploadService uploadService, IUtilities utilities)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _uploadService = uploadService;
            _utilities = utilities;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userForm = new ProfileEditViewModel()
            {
                Email = user.Email,
                FullName = user.FullName
            };

            ViewBag.ChangeSuccess = false;
            return View(userForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileEditViewModel viewModel)
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
                        ViewBag.ChangeSuccess = false;
                        return View(viewModel);
                    }
                }

                user.FullName = viewModel.FullName;
                user.PhotoUrl = url;
                _db.UserRepository.Update(user);
                await _db.SaveAsync();

                ViewBag.ChangeSuccess = true;
                return View(viewModel);
            }
            else
            {
                ViewBag.ChangeSuccess = false;
                return View(viewModel);
            }
        }
        #endregion

        #region ChangePassword
        public IActionResult ChangePassword()
        {
            ViewBag.ChangeSuccess = false;
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
                    ViewBag.ChangeSuccess = true;
                    return View();
                }
                else
                {
                    //foreach (var errorMessage in result.Errors.ToList())
                    //{
                    //    ModelState.AddModelError("", errorMessage.Description);
                    //}
                    ModelState.AddModelError("", "خطا در تغییر کلمه عبور");
                    ViewBag.ChangeSuccess = false;
                    return View(viewModel);
                }
            }
            else
            {
                ViewBag.ChangeSuccess = false;
                return View(viewModel);
            }
        }
        #endregion
    }
}
