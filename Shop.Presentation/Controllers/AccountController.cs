using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Account;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IAccountService _accountService;

        public AccountController(IUnitOfWork<DatabaseContext> db, IAccountService accountService)
        {
            _db = db;
            _accountService = accountService;
        }

        #region Register
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _db.UserRepository.UserExistsAsync(viewModel.Email))
                {
                    ModelState.AddModelError("EMail", "این ایمیل قبلا ثبت شده است");
                    return View(viewModel);
                }
                else
                {
                    var result = await _accountService.Register(viewModel);
                    if (result.Status)
                    {
                        ViewBag.SuccessRegister = true;
                        return View();
                    }
                    else
                    {
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
                return View(viewModel);
            }
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Login(viewModel);
                if (result.Status)
                {
                    //ToDo redirect to panel
                    return RedirectToAction("/");
                }
                else
                {
                    foreach (var errorMessage in result.ErrorMessages)
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                    return View(viewModel);
                }
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion
    }
}
