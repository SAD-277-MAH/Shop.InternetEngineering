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
        public async Task<IActionResult> Add(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(viewModel);
                await _db.CategoryRepository.AddAsync(category);
                await _db.SaveAsync();

                return Redirect("/Admin/Category");
            }

            return View(viewModel);
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

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = await _db.CategoryRepository.GetAsync(id);

                if (category != null)
                {
                    _mapper.Map<CategoryViewModel, Category>(viewModel, category);

                    _db.CategoryRepository.Update(category);
                    await _db.SaveAsync();

                    return Redirect("/Admin/Category");
                }

                return NotFound();
            }

            return View(viewModel);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var category = await _db.CategoryRepository.GetAsync(id);

                if (category != null)
                {
                    return View();
                }

                return NotFound();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.CategoryRepository.GetAsync(id);

            if (category != null)
            {
                _db.CategoryRepository.Delete(id);
                await _db.SaveAsync();

                return Redirect("/Admin/Category");
            }

            return NotFound();
        }
        #endregion
    }
}
