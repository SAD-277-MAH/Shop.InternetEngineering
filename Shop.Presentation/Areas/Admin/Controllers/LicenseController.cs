using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "RequireAdminRole")]
    public class LicenseController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public LicenseController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var licenses = await _db.LicenseRepository.GetAsync();

            return View(licenses);
        }
        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(LicenseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var license = _mapper.Map<License>(viewModel);
                await _db.LicenseRepository.AddAsync(license);
                await _db.SaveAsync();

                return Redirect("/Admin/License");
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id)
        {
            var license = await _db.LicenseRepository.GetAsync(id);

            if (license != null)
            {
                var licenseViewModel = _mapper.Map<LicenseViewModel>(license);
                return View(licenseViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LicenseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var license = await _db.LicenseRepository.GetAsync(id);

                if (license != null)
                {
                    _mapper.Map(viewModel, license);

                    _db.LicenseRepository.Update(license);
                    await _db.SaveAsync();

                    return Redirect("/Admin/License");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return View(viewModel);
            }
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var license = await _db.LicenseRepository.GetAsync(id);

            if (license != null)
            {
                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var license = await _db.LicenseRepository.GetAsync(id);

            if (license != null)
            {
                _db.LicenseRepository.Delete(id);
                await _db.SaveAsync();

                return Redirect("/Admin/License");
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
