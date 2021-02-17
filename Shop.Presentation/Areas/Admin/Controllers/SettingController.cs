using AutoMapper;
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
    public class SettingController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public SettingController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Site
        public async Task<IActionResult> Site()
        {
            var setting = (await _db.SettingRepository.GetAsync()).LastOrDefault();

            if (setting == null)
            {
                setting = new Setting();
                await _db.SettingRepository.AddAsync(setting);
                await _db.SaveAsync();
            }

            var siteSetting = _mapper.Map<SiteSettingViewModel>(setting);

            ViewBag.ChangeSuccess = false;
            return View(siteSetting);
        }

        [HttpPost]
        public async Task<IActionResult> Site(SiteSettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var setting = (await _db.SettingRepository.GetAsync()).LastOrDefault();
                _mapper.Map(viewModel, setting);
                _db.SettingRepository.Update(setting);
                await _db.SaveAsync();

                ViewBag.ChangeSuccess = true;
                return View(viewModel);
            }

            ViewBag.ChangeSuccess = false;
            return View();
        }
        #endregion

        #region MessageSender
        public async Task<IActionResult> MessageSender()
        {
            var setting = (await _db.SettingRepository.GetAsync()).LastOrDefault();

            if (setting == null)
            {
                setting = new Setting();
                await _db.SettingRepository.AddAsync(setting);
                await _db.SaveAsync();
            }

            var messageSenderSetting = _mapper.Map<MessageSenderSettingViewModel>(setting);

            ViewBag.ChangeSuccess = false;
            return View(messageSenderSetting);
        }

        [HttpPost]
        public async Task<IActionResult> MessageSender(MessageSenderSettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var setting = (await _db.SettingRepository.GetAsync()).LastOrDefault();
                _mapper.Map(viewModel, setting);
                _db.SettingRepository.Update(setting);
                await _db.SaveAsync();

                ViewBag.ChangeSuccess = true;
                return View(viewModel);
            }

            ViewBag.ChangeSuccess = false;
            return View();
        }
        #endregion
    }
}
