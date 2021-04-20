using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Panel;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Policy = "RequireUserRole")]
    public class AddressController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AddressController(IUnitOfWork<DatabaseContext> db, IMapper mapper, UserManager<User> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var addresses = await _db.AddressRepository.GetAsync(a => a.UserId == user.Id, null, string.Empty);
            var returnAddresses = _mapper.Map<List<AddressViewModel>>(addresses);

            return View(returnAddresses);
        }
        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddressAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var address = _mapper.Map<Address>(viewModel);
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                address.UserId = userId;
                await _db.AddressRepository.AddAsync(address);
                await _db.SaveAsync();

                return Redirect("/Panel/Address");
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(string id)
        {
            var address = await _db.AddressRepository.GetAsync(id);
            if (address != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                if (address.UserId == userId)
                {
                    var returnAddress = _mapper.Map<AddressViewModel>(address);
                    return View(returnAddress);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AddressViewModel viewModel)
        {
            var address = await _db.AddressRepository.GetAsync(id);
            if (address != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                if (address.UserId == userId)
                {
                    if (ModelState.IsValid)
                    {
                        _mapper.Map(viewModel, address);
                        _db.AddressRepository.Update(address);
                        await _db.SaveAsync();

                        return Redirect("/Panel/Address");
                    }
                    else
                    {
                        return View(viewModel);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(string id)
        {
            var address = await _db.AddressRepository.GetAsync(id);
            if (address != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                if (address.UserId == userId)
                {
                    return View();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var address = await _db.AddressRepository.GetAsync(id);
            if (address != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                if (address.UserId == userId)
                {
                    if (ModelState.IsValid)
                    {
                        _db.AddressRepository.Delete(address);
                        await _db.SaveAsync();

                        return Redirect("/Panel/Address");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
