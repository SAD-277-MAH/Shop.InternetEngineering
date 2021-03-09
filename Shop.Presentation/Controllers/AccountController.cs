using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Helpers.Interface;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Account;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Shop.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMessageSender _messageSender;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(IUnitOfWork<DatabaseContext> db, IAccountService accountService, UserManager<User> userManager, SignInManager<User> signInManager, IMessageSender messageSender, IViewRenderService viewRenderService)
        {
            _db = db;
            _accountService = accountService;
            _userManager = userManager;
            _signInManager = signInManager;
            _messageSender = messageSender;
            _viewRenderService = viewRenderService;
        }

        #region Register
        [Route("Register")]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin"))
                {
                    return Redirect("/Admin");
                }
                else
                {
                    return Redirect("/Panel");
                }
            }
            ViewBag.SuccessRegister = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _db.UserRepository.UserExistsAsync(viewModel.Email))
                {
                    ViewBag.SuccessRegister = false;

                    ModelState.AddModelError("Email", "این ایمیل قبلا ثبت شده است");
                    return View(viewModel);
                }
                else
                {
                    var result = await _accountService.Register(viewModel);
                    if (result.Status)
                    {
                        ViewBag.SuccessRegister = true;

                        // Send Email
                        var token = await _accountService.GetActivateEmailToken(result.User);
                        var activateModel = new EmailUrlViewModel()
                        {
                            Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + Url.Action(nameof(ActivateEmail), new { UserName = result.User.UserName, Token = token })
                        };
                        string emailBody = _viewRenderService.RenderToString("_ActivateEmail", activateModel);
                        _messageSender.SendEmail(result.User.Email, "فعالسازی حساب کاربری", emailBody);

                        return View();
                    }
                    else
                    {
                        ViewBag.SuccessRegister = false;

                        foreach (var errorMessage in result.ErrorMessages)
                        {
                            ModelState.AddModelError("", errorMessage);
                        }
                        return View(viewModel);
                    }
                }
            }
            else
            {
                ViewBag.SuccessRegister = false;

                return View(viewModel);
            }
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login(string ReturnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin"))
                {
                    return Redirect("/Admin");
                }
                else
                {
                    return Redirect("/Panel");
                }
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Login(viewModel);
                if (result.Status)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(result.User, "Admin"))
                        {
                            return Redirect("/Admin");
                        }
                        else
                        {
                            return Redirect("/Panel");
                        }
                    }
                }
                else
                {
                    foreach (var errorMessage in result.ErrorMessages)
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                    ViewData["ReturnUrl"] = ReturnUrl;
                    return View(viewModel);
                }
            }
            else
            {
                ViewData["ReturnUrl"] = ReturnUrl;
                return View(viewModel);
            }
        }
        #endregion

        #region ActivateEmail
        [Route("ActivateEmail")]
        public async Task<IActionResult> ActivateEmail(string UserName, string Token)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Token))
            {
                return NotFound();
            }

            var result = await _accountService.ActivateEmail(UserName, Token);
            if (result)
            {
                ViewBag.SuccessActivate = true;

                return View();
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region ResendEmail
        public IActionResult ResendEmail()
        {
            ViewBag.SuccessResendEmail = 0;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmail(SendEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.UserRepository.GetAsync(u => u.NormalizedUserName == viewModel.Email.ToUpper(), string.Empty);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        ViewBag.SuccessResendEmail = -1;

                        return View();
                    }
                    ViewBag.SuccessResendEmail = 1;

                    // Send Email
                    var token = await _accountService.GetActivateEmailToken(user);
                    var activateModel = new EmailUrlViewModel()
                    {
                        Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + Url.Action(nameof(ActivateEmail), new { UserName = user.UserName, Token = token })
                    };
                    string emailBody = _viewRenderService.RenderToString("_ActivateEmail", activateModel);
                    _messageSender.SendEmail(user.Email, "فعالسازی حساب کاربری", emailBody);

                    return View();
                }
                else
                {
                    ViewBag.SuccessResendEmail = 0;

                    return View();
                }
            }
            else
            {
                ViewBag.SuccessResendEmail = 0;

                return View(viewModel);
            }
        }
        #endregion

        #region ForgetPassword
        public IActionResult ForgetPassword()
        {
            ViewBag.SuccessForgetPassword = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(SendEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.UserRepository.GetAsync(u => u.NormalizedUserName == viewModel.Email.ToUpper(), string.Empty);
                if (user != null)
                {
                    ViewBag.SuccessForgetPassword = true;

                    // Send Email
                    var token = await _accountService.GetChangePasswordToken(user);
                    var activateModel = new EmailUrlViewModel()
                    {
                        Url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + Url.Action(nameof(ResetPassword), new { UserName = user.UserName, Token = HttpUtility.UrlEncode(token) })
                    };
                    string emailBody = _viewRenderService.RenderToString("_ForgetPasswordEmail", activateModel);
                    _messageSender.SendEmail(user.Email, "فراموشی رمز عبور", emailBody);

                    return View();
                }
                else
                {
                    ViewBag.SuccessForgetPassword = false;

                    return View();
                }
            }
            else
            {
                ViewBag.SuccessForgetPassword = false;

                return View(viewModel);
            }
        }
        #endregion

        #region ResetPassword
        //[Route("ResetPassword")]
        //public async Task<IActionResult> ResetPassword(string UserName, string Token)
        //{
        //    if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Token))
        //    {
        //        return NotFound();
        //    }

        //    var result = await _accountService.ResetPassword(UserName, Token);
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        ViewBag.SuccessResetPassword = true;

        //        // Send Email
        //        var passwordModel = new ResetPasswordViewModel()
        //        {
        //            Value = result
        //        };
        //        string emailBody = _viewRenderService.RenderToString("_ResetPasswordEmail", passwordModel);
        //        _messageSender.SendEmail(UserName, "تغییر کلمه عبور", emailBody);

        //        return View();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}
        public IActionResult ResetPassword(string UserName, string Token)
        {
            ViewData["UserName"] = UserName;
            ViewData["Token"] = Token;
            ViewBag.SuccessResetPassword = false;
            return View();
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordConfirm([FromQuery] string UserName, [FromQuery] string Token, SetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Token))
                {
                    return NotFound();
                }

                var user = await _userManager.FindByNameAsync(UserName);
                var result = await _userManager.ResetPasswordAsync(user, Token, viewModel.Password);
                if (result.Succeeded)
                {
                    ViewData["UserName"] = UserName;
                    ViewData["Token"] = Token;
                    ViewBag.SuccessResetPassword = true;
                    return View();
                }
                else
                {
                    ViewData["UserName"] = UserName;
                    ViewData["Token"] = Token;
                    ViewBag.SuccessResetPassword = false;
                    ModelState.AddModelError("", "خطا در تغییر رمز عبور");
                    return View(viewModel);
                }
            }
            else
            {
                ViewData["UserName"] = UserName;
                ViewData["Token"] = Token;
                ViewBag.SuccessResetPassword = false;
                return View(viewModel);
            }
        }
        #endregion

        #region Logout
        //[HttpPost()]
        //[ValidateAntiForgeryToken]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
        #endregion
    }
}
