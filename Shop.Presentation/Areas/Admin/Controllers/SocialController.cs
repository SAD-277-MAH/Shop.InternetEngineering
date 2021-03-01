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
    public class SocialController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public SocialController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var socials = await _db.SocialRepository.GetAsync();

            return View(socials);
        }
        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SocialViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var social = _mapper.Map<Social>(viewModel);
                await _db.SocialRepository.AddAsync(social);
                await _db.SaveAsync();

                return Redirect("/Admin/Social");
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
            var social = await _db.SocialRepository.GetAsync(id);

            if (social != null)
            {
                var socialViewModel = _mapper.Map<SocialViewModel>(social);
                return View(socialViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SocialViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var social = await _db.SocialRepository.GetAsync(id);

                if (social != null)
                {
                    _mapper.Map(viewModel, social);

                    _db.SocialRepository.Update(social);
                    await _db.SaveAsync();

                    return Redirect("/Admin/Social");
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
            var social = await _db.SocialRepository.GetAsync(id);

            if (social != null)
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
            var social = await _db.SocialRepository.GetAsync(id);

            if (social != null)
            {
                _db.SocialRepository.Delete(id);
                await _db.SaveAsync();

                return Redirect("/Admin/Social");
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
