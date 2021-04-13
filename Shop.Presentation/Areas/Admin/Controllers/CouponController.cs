using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly ICodeGenerator _codeGenerator;

        public CouponController(IUnitOfWork<DatabaseContext> db, IMapper mapper, ICodeGenerator codeGenerator)
        {
            _db = db;
            _mapper = mapper;
            _codeGenerator = codeGenerator;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var coupons = await _db.CouponRepository.GetAsync();

            return View(coupons);
        }

        public async Task<IActionResult> Details(string id)
        {
            var coupon = await _db.CouponRepository.GetAsync(p => p.Id == id, "CouponUsers,CouponProducts,CouponCategories");
            if (coupon != null)
            {
                var couponDetail = new CouponDetailViewModel();
                #region CouponDetail
                PersianCalendar pc = new PersianCalendar();

                couponDetail.Title = coupon.Title;
                couponDetail.Type = coupon.Type ? "درصدی" : "مقداری";
                couponDetail.Value = coupon.Value.ToString();
                couponDetail.HasCountLimit = coupon.HasCountLimit ? "دارد" : "ندارد";
                couponDetail.CountLimit = coupon.HasCountLimit ? coupon.CountLimit.ToString() : "*بدون محدودیت*";
                couponDetail.HasDateLimit = coupon.HasDateLimit ? "دارد" : "ندارد";
                couponDetail.StartDateLimit = coupon.HasDateLimit ? $"{pc.GetYear(coupon.StartDateLimit)}/{pc.GetMonth(coupon.StartDateLimit)}/{pc.GetDayOfMonth(coupon.StartDateLimit)}" : "*بدون محدودیت*";
                couponDetail.EndDateLimit = coupon.HasDateLimit ? $"{pc.GetYear(coupon.EndDateLimit)}/{pc.GetMonth(coupon.EndDateLimit)}/{pc.GetDayOfMonth(coupon.EndDateLimit)}" : "*بدون محدودیت*";
                couponDetail.HasUserLimit = coupon.HasUserLimit ? "دارد" : "ندارد";
                couponDetail.Users = new List<string>();
                if (coupon.HasUserLimit)
                {
                    foreach (var couponUser in coupon.CouponUsers)
                    {
                        var _user = await _db.UserRepository.GetAsync(couponUser.UserId);
                        if (_user != null)
                        {
                            couponDetail.Users.Add(_user.UserName);
                        }
                    }
                }
                else
                {
                    couponDetail.Users.Add("*بدون محدودیت*");
                }
                couponDetail.HasProductLimit = coupon.HasProductLimit ? "دارد" : "ندارد";
                couponDetail.Products = new List<string>();
                if (coupon.HasProductLimit)
                {
                    foreach (var couponProduct in coupon.CouponProducts)
                    {
                        var _product = await _db.ProductRepository.GetAsync(couponProduct.ProductId);
                        if (_product != null)
                        {
                            couponDetail.Products.Add(_product.Name);
                        }
                    }
                }
                else
                {
                    couponDetail.Products.Add("*بدون محدودیت*");
                }
                couponDetail.HasCategoryLimit = coupon.HasCategoryLimit ? "دارد" : "ندارد";
                couponDetail.Categories = new List<string>();
                if (coupon.HasCategoryLimit)
                {
                    foreach (var couponCategory in coupon.CouponCategories)
                    {
                        var _category = await _db.CategoryRepository.GetAsync(couponCategory.CategoryId);
                        if (_category != null)
                        {
                            couponDetail.Categories.Add(_category.Name);
                        }
                    }
                }
                else
                {
                    couponDetail.Categories.Add("*بدون محدودیت*");
                }
                #endregion

                return View(couponDetail);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Add
        public async Task<IActionResult> Add()
        {
            #region SelectList
            var users = await _db.UserRepository.GetAsync();
            ViewData["UsersId"] = new SelectList(users, "Id", "UserName");

            var products = await _db.ProductRepository.GetAsync();
            ViewData["ProductsId"] = new SelectList(products, "Id", "Name");

            var categories = await _db.CategoryRepository.GetAsync();
            ViewData["CategoriesId"] = new SelectList(categories, "Id", "Name");
            #endregion
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CouponViewModel viewModel)
        {
            #region SelectList
            var users = await _db.UserRepository.GetAsync();
            ViewData["UsersId"] = new SelectList(users, "Id", "UserName");

            var products = await _db.ProductRepository.GetAsync();
            ViewData["ProductsId"] = new SelectList(products, "Id", "Name");

            var categories = await _db.CategoryRepository.GetAsync();
            ViewData["CategoriesId"] = new SelectList(categories, "Id", "Name");
            #endregion
            if (ModelState.IsValid)
            {
                viewModel.Title = viewModel.Title.Trim();
                var couponModel = await _db.CouponRepository.GetAsync(c => c.Title == viewModel.Title, string.Empty);
                if (couponModel == null)
                {
                    if (viewModel.EndDateLimit < viewModel.StartDateLimit)
                    {
                        ModelState.AddModelError("EndDateLimit", "تاریخ پایان باید بعد از تاریخ شروع باشد");
                        return View(viewModel);
                    }

                    if (viewModel.HasUserLimit && viewModel.Users.Count == 0)
                    {
                        ModelState.AddModelError("Users", "کاربران را وارد کنید");
                        return View(viewModel);
                    }

                    if (viewModel.HasProductOrCategoryLimit)
                    {
                        if (viewModel.ProductOrCategoryLimit == "product" && viewModel.Products.Count == 0)
                        {
                            ModelState.AddModelError("Products", "محصولات را وارد کنید");
                            return View(viewModel);
                        }

                        if (viewModel.ProductOrCategoryLimit == "category" && viewModel.Categories.Count == 0)
                        {
                            ModelState.AddModelError("Categories", "دسته بندی ها را وارد کنید");
                            return View(viewModel);
                        }
                    }

                    var coupon = new Coupon();
                    #region Coupon
                    coupon.Title = viewModel.Title.Trim();
                    coupon.Type = viewModel.Type == "percent" ? true : false;
                    coupon.Value = viewModel.Type == "percent" ? viewModel.PercentDiscount : viewModel.ValueDiscount;
                    coupon.HasCountLimit = viewModel.HasCountLimit;
                    if (viewModel.HasCountLimit)
                    {
                        coupon.CountLimit = viewModel.CountLimit;
                    }
                    coupon.HasDateLimit = viewModel.HasDateLimit;
                    if (viewModel.HasDateLimit)
                    {
                        coupon.StartDateLimit = viewModel.StartDateLimit;
                        coupon.EndDateLimit = viewModel.EndDateLimit;
                    }
                    coupon.HasUserLimit = viewModel.HasUserLimit;
                    if (viewModel.HasProductOrCategoryLimit)
                    {
                        if (viewModel.ProductOrCategoryLimit == "product")
                        {
                            coupon.HasProductLimit = true;
                            coupon.HasCategoryLimit = false;
                        }
                        else
                        {
                            coupon.HasCategoryLimit = true;
                            coupon.HasProductLimit = false;
                        }
                    }
                    coupon.Code = await _codeGenerator.GenerateCouponCodeAsync();
                    #endregion

                    await _db.CouponRepository.AddAsync(coupon);
                    await _db.SaveAsync();

                    if (viewModel.HasUserLimit)
                    {
                        foreach (var userId in viewModel.Users)
                        {
                            var couponUser = new CouponUser()
                            {
                                CouponId = coupon.Id,
                                UserId = userId
                            };
                            await _db.CouponUserRepository.AddAsync(couponUser);
                        }
                        await _db.SaveAsync();
                    }
                    if (viewModel.HasProductOrCategoryLimit)
                    {
                        if (viewModel.ProductOrCategoryLimit == "product")
                        {
                            foreach (var productId in viewModel.Products)
                            {
                                var couponProduct = new CouponProduct()
                                {
                                    CouponId = coupon.Id,
                                    ProductId = productId
                                };
                                await _db.CouponProductRepository.AddAsync(couponProduct);
                            }
                            await _db.SaveAsync();
                        }
                        else
                        {
                            foreach (var categoryId in viewModel.Categories)
                            {
                                var couponCategory = new CouponCategory()
                                {
                                    CouponId = coupon.Id,
                                    CategoryId = categoryId
                                };
                                await _db.CouponCategoryRepository.AddAsync(couponCategory);
                            }
                            await _db.SaveAsync();
                        }
                    }

                    return Redirect("/Admin/Coupon");
                }
                else
                {
                    ModelState.AddModelError("Title", "کد تخفیف با این نام قبلا ثبت شده است");
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
        public async Task<IActionResult> Edit(string id)
        {
            var coupon = await _db.CouponRepository.GetAsync(id);

            if (coupon != null)
            {
                var couponViewModel = new CouponViewModel();
                #region couponViewModel
                couponViewModel.Title = coupon.Title;
                couponViewModel.Type = coupon.Type ? "percent" : "value";
                if (coupon.Type)
                {
                    couponViewModel.PercentDiscount = coupon.Value;
                    couponViewModel.ValueDiscount = 1;
                }
                else
                {
                    couponViewModel.ValueDiscount = coupon.Value;
                    couponViewModel.PercentDiscount = 1;
                }
                couponViewModel.HasCountLimit = coupon.HasCountLimit;
                if (coupon.HasCountLimit)
                {
                    couponViewModel.CountLimit = coupon.CountLimit;
                }
                else
                {
                    couponViewModel.CountLimit = 1;
                }
                couponViewModel.HasDateLimit = coupon.HasDateLimit;
                if (coupon.HasDateLimit)
                {
                    couponViewModel.StartDateLimit = coupon.StartDateLimit;
                    couponViewModel.EndDateLimit = coupon.EndDateLimit;
                }
                else
                {
                    couponViewModel.StartDateLimit = DateTime.Now;
                    couponViewModel.EndDateLimit = DateTime.Now;
                }
                couponViewModel.HasUserLimit = coupon.HasUserLimit;
                if (coupon.HasUserLimit)
                {
                    var couponUsers = await _db.CouponUserRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                    couponViewModel.Users = new List<string>();
                    foreach (var couponUser in couponUsers)
                    {
                        couponViewModel.Users.Add(couponUser.UserId);
                    }
                }
                if (coupon.HasProductLimit || coupon.HasCategoryLimit)
                {
                    couponViewModel.HasProductOrCategoryLimit = true;
                    if (coupon.HasProductLimit)
                    {
                        couponViewModel.ProductOrCategoryLimit = "product";

                        var couponProducts = await _db.CouponProductRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                        couponViewModel.Products = new List<string>();
                        foreach (var couponProduct in couponProducts)
                        {
                            couponViewModel.Products.Add(couponProduct.ProductId);
                        }
                    }
                    else
                    {
                        couponViewModel.ProductOrCategoryLimit = "category";

                        var couponCategories = await _db.CouponCategoryRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                        couponViewModel.Categories = new List<int>();
                        foreach (var couponCategory in couponCategories)
                        {
                            couponViewModel.Categories.Add(couponCategory.CategoryId);
                        }
                    }
                }
                else
                {
                    couponViewModel.HasProductOrCategoryLimit = false;
                    couponViewModel.ProductOrCategoryLimit = "product";
                }
                #endregion

                #region SelectList
                var users = await _db.UserRepository.GetAsync();
                ViewData["UsersId"] = new SelectList(users, "Id", "UserName");

                var products = await _db.ProductRepository.GetAsync();
                ViewData["ProductsId"] = new SelectList(products, "Id", "Name");

                var categories = await _db.CategoryRepository.GetAsync();
                ViewData["CategoriesId"] = new SelectList(categories, "Id", "Name");
                #endregion
                return View(couponViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CouponViewModel viewModel)
        {
            #region SelectList
            var users = await _db.UserRepository.GetAsync();
            ViewData["UsersId"] = new SelectList(users, "Id", "UserName");

            var products = await _db.ProductRepository.GetAsync();
            ViewData["ProductsId"] = new SelectList(products, "Id", "Name");

            var categories = await _db.CategoryRepository.GetAsync();
            ViewData["CategoriesId"] = new SelectList(categories, "Id", "Name");
            #endregion
            if (ModelState.IsValid)
            {
                viewModel.Title = viewModel.Title.Trim();
                var couponModel = await _db.CouponRepository.GetAsync(c => c.Title == viewModel.Title.Trim() && c.Id != id, string.Empty);
                if (couponModel == null)
                {
                    var coupon = await _db.CouponRepository.GetAsync(id);

                    if (coupon != null)
                    {
                        if (viewModel.EndDateLimit < viewModel.StartDateLimit)
                        {
                            ModelState.AddModelError("EndDateLimit", "تاریخ پایان باید بعد از تاریخ شروع باشد");
                            return View(viewModel);
                        }

                        if (viewModel.HasUserLimit && viewModel.Users.Count == 0)
                        {
                            ModelState.AddModelError("Users", "کاربران را وارد کنید");
                            return View(viewModel);
                        }

                        if (viewModel.HasProductOrCategoryLimit)
                        {
                            if (viewModel.ProductOrCategoryLimit == "product" && viewModel.Products.Count == 0)
                            {
                                ModelState.AddModelError("Products", "محصولات را وارد کنید");
                                return View(viewModel);
                            }

                            if (viewModel.ProductOrCategoryLimit == "category" && viewModel.Categories.Count == 0)
                            {
                                ModelState.AddModelError("Categories", "دسته بندی ها را وارد کنید");
                                return View(viewModel);
                            }
                        }

                        #region Coupon
                        coupon.Title = viewModel.Title.Trim();
                        coupon.Type = viewModel.Type == "percent" ? true : false;
                        coupon.Value = viewModel.Type == "percent" ? viewModel.PercentDiscount : viewModel.ValueDiscount;
                        coupon.HasCountLimit = viewModel.HasCountLimit;
                        if (viewModel.HasCountLimit)
                        {
                            coupon.CountLimit = viewModel.CountLimit;
                        }
                        coupon.HasDateLimit = viewModel.HasDateLimit;
                        if (viewModel.HasDateLimit)
                        {
                            coupon.StartDateLimit = viewModel.StartDateLimit;
                            coupon.EndDateLimit = viewModel.EndDateLimit;
                        }
                        coupon.HasUserLimit = viewModel.HasUserLimit;
                        if (viewModel.HasProductOrCategoryLimit)
                        {
                            if (viewModel.ProductOrCategoryLimit == "product")
                            {
                                coupon.HasProductLimit = true;
                                coupon.HasCategoryLimit = false;
                            }
                            else
                            {
                                coupon.HasCategoryLimit = true;
                                coupon.HasProductLimit = false;
                            }
                        }
                        #endregion

                        _db.CouponRepository.Update(coupon);
                        await _db.SaveAsync();

                        if (viewModel.HasUserLimit)
                        {
                            var oldUsers = await _db.CouponUserRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                            foreach (var oldUser in oldUsers)
                            {
                                _db.CouponUserRepository.Delete(oldUser);
                            }
                            foreach (var userId in viewModel.Users)
                            {
                                var couponUser = new CouponUser()
                                {
                                    CouponId = coupon.Id,
                                    UserId = userId
                                };
                                await _db.CouponUserRepository.AddAsync(couponUser);
                            }
                            await _db.SaveAsync();
                        }
                        if (viewModel.HasProductOrCategoryLimit)
                        {
                            if (viewModel.ProductOrCategoryLimit == "product")
                            {
                                var oldProducts = await _db.CouponProductRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                                foreach (var oldProduct in oldProducts)
                                {
                                    _db.CouponProductRepository.Delete(oldProduct);
                                }
                                foreach (var productId in viewModel.Products)
                                {
                                    var couponProduct = new CouponProduct()
                                    {
                                        CouponId = coupon.Id,
                                        ProductId = productId
                                    };
                                    await _db.CouponProductRepository.AddAsync(couponProduct);
                                }
                                await _db.SaveAsync();
                            }
                            else
                            {
                                var oldCategories = await _db.CouponCategoryRepository.GetAsync(c => c.CouponId == coupon.Id, null, string.Empty);
                                foreach (var oldCategory in oldCategories)
                                {
                                    _db.CouponCategoryRepository.Delete(oldCategory);
                                }
                                foreach (var categoryId in viewModel.Categories)
                                {
                                    var couponCategory = new CouponCategory()
                                    {
                                        CouponId = coupon.Id,
                                        CategoryId = categoryId
                                    };
                                    await _db.CouponCategoryRepository.AddAsync(couponCategory);
                                }
                                await _db.SaveAsync();
                            }
                        }

                        return Redirect("/Admin/Coupon");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    ModelState.AddModelError("Title", "کد تخفیف با این نام قبلا ثبت شده است");
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
