using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Policy = "RequireAdminRole")]
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
                    return Redirect("/Admin/UsersManagement");
                }
                else
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, RoleName);

                    return Redirect("/Admin/UsersManagement");
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
    }
}
