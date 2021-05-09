using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.ViewModels.Site;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel();

            var resultCategories = await _db.CategoryRepository.GetAsync();
            var categories = _mapper.Map<List<SiteCategoryViewModel>>(resultCategories);
            viewModel.Categories = categories;

            var products = await _db.ProductRepository.GetAsync(null, null, "OrderDetails");

            var mostDiscount = products.Where(p => p.Discount > 0).OrderByDescending(p => p.Discount).Take(10);
            viewModel.MostDiscountProducts = _mapper.Map<List<ProductCartViewModel>>(mostDiscount);

            var mostPurchased = products.OrderByDescending(p => p.OrderDetails.Count).Take(10);
            viewModel.MostPurchasedProducts = _mapper.Map<List<ProductCartViewModel>>(mostPurchased);

            var mostRecent = products.OrderByDescending(p => p.DateCreated).Take(10).ToList();
            viewModel.MostRecentProducts = _mapper.Map<List<ProductCartViewModel>>(mostRecent);

            return View(viewModel);
        }
        #endregion

        #region Product
        [Route("Product/{id}/{title}")]
        public async Task<IActionResult> Product(string id, string? title)
        {
            var resultProduct = await _db.ProductRepository.GetAsync(p => p.Id == id, "Category,ProductImages");
            if (resultProduct == null)
            {
                return NotFound();
            }

            var product = _mapper.Map<ProductPageViewModel>(resultProduct);

            return View(product);
        }
        #endregion

        #region Shop
        public async Task<IActionResult> Shop(string search, string category)
        {
            var products = await _db.ProductRepository.GetAsync(null, null, "Category");

            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products.Where(p => p.Category.Name == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower().Trim();
                products = products.Where(p => p.Name.Contains(search) || p.Category.Name.Contains(search) || p.BrandName.Contains(search) || p.Description.Contains(search));
            }

            var result = _mapper.Map<List<ProductCartViewModel>>(products.OrderByDescending(p => p.DateCreated));

            return View(result);
        }
        #endregion
    }
}
