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
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var categories = await _db.CategoryRepository.GetAsync();

            return View(categories);
        }
        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var categoryModel = await _db.CategoryRepository.GetAsync(c => c.Name == viewModel.Name, string.Empty);
                if (categoryModel == null)
                {
                    var category = _mapper.Map<Category>(viewModel);
                    await _db.CategoryRepository.AddAsync(category);
                    await _db.SaveAsync();

                    return Redirect("/Admin/Category");
                }
                else
                {
                    ModelState.AddModelError("Name", "دسته بندی با این نام قبلا ثبت شده است");
                    return View(viewModel);
                }
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
            var category = await _db.CategoryRepository.GetAsync(id);

            if (category != null)
            {
                var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                return View(categoryViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var categoryModel = await _db.CategoryRepository.GetAsync(c => c.Name == viewModel.Name && c.Id != id, string.Empty);
                if (categoryModel == null)
                {
                    var category = await _db.CategoryRepository.GetAsync(id);

                    if (category != null)
                    {
                        _mapper.Map(viewModel, category);

                        _db.CategoryRepository.Update(category);
                        await _db.SaveAsync();

                        return Redirect("/Admin/Category");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    ModelState.AddModelError("Name", "دسته بندی با این نام قبلا ثبت شده است");
                    return View(viewModel);
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
            var category = await _db.CategoryRepository.GetAsync(id);

            if (category != null)
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
            var category = await _db.CategoryRepository.GetAsync(id);

            if (category != null)
            {
                _db.CategoryRepository.Delete(id);
                await _db.SaveAsync();

                return Redirect("/Admin/Category");
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
