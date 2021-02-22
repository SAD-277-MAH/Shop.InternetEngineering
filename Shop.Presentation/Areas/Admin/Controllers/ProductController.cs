using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Common.Extentions;
using Shop.Common.Helpers.Interface;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using Shop.Services.Upload.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Policy = "RequireAdminRole")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;
        private readonly IUtilities _utilities;

        public ProductController(IUnitOfWork<DatabaseContext> db, IMapper mapper, IUploadService uploadService, IUtilities utilities)
        {
            _db = db;
            _mapper = mapper;
            _uploadService = uploadService;
            _utilities = utilities;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var products = await _db.ProductRepository.GetAsync(null, null, "Category");
            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _db.ProductRepository.GetAsync(p => p.Id == id, "Category");
            if (product != null)
            {
                return View(product);
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
            var categories = await _db.CategoryRepository.GetAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.File.IsImage())
                {
                    PersianCalendar pc = new PersianCalendar();
                    var uploadResult = await _uploadService.UploadFile(viewModel.File, string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value), "Products\\" + pc.GetYear(DateTime.Now) + "\\" + pc.GetMonth(DateTime.Now) + "\\" + pc.GetDayOfMonth(DateTime.Now));

                    if (uploadResult.Status)
                    {
                        var product = _mapper.Map<Product>(viewModel);
                        product.PhotoUrl = uploadResult.Url;

                        await _db.ProductRepository.AddAsync(product);
                        await _db.SaveAsync();

                        return Redirect("/Admin/Product");
                    }
                    else
                    {
                        ModelState.AddModelError("File", "خطا در آپلود تصویر محصول");
                        var categories = await _db.CategoryRepository.GetAsync();
                        ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                        return View(viewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "تصویر محصول معتبر نیست");
                    var categories = await _db.CategoryRepository.GetAsync();
                    ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                    return View(viewModel);
                }
            }
            else
            {
                var categories = await _db.CategoryRepository.GetAsync();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                return View(viewModel);
            }
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _db.ProductRepository.GetAsync(id);
            if (product != null)
            {
                var categories = await _db.CategoryRepository.GetAsync();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", product.CategoryId);

                var returnProduct = _mapper.Map<ProductEditViewModel>(product);
                returnProduct.ProductPhotoUrl = product.PhotoUrl;

                return View(returnProduct);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = await _db.ProductRepository.GetAsync(id);
                if (product != null)
                {
                    string url = product.PhotoUrl;
                    if (viewModel.File.IsImage())
                    {
                        PersianCalendar pc = new PersianCalendar();
                        var uploadResult = await _uploadService.UploadFile(viewModel.File, string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value), "Products\\" + pc.GetYear(DateTime.Now) + "\\" + pc.GetMonth(DateTime.Now) + "\\" + pc.GetDayOfMonth(DateTime.Now));

                        if (uploadResult.Status)
                        {
                            try
                            {
                                _uploadService.RemoveFileFromLocal(_utilities.FindLocalPathFromUrl(url));
                            }
                            catch { }

                            url = uploadResult.Url;
                        }
                        else
                        {
                            ModelState.AddModelError("File", "خطا در آپلود تصویر محصول");
                            var categories = await _db.CategoryRepository.GetAsync();
                            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                            return View(viewModel);
                        }
                    }

                    _mapper.Map(viewModel, product);
                    product.PhotoUrl = url;
                    _db.ProductRepository.Update(product);
                    await _db.SaveAsync();

                    return Redirect("/Admin/Product");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                var categories = await _db.CategoryRepository.GetAsync();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", viewModel.CategoryId);

                return View(viewModel);
            }
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                var product = await _db.ProductRepository.GetAsync(id);

                if (product != null)
                {
                    return View();
                }
                else
                {
                    return NotFound();
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
            var product = await _db.ProductRepository.GetAsync(id);

            if (product != null)
            {
                _db.ProductRepository.Delete(id);
                await _db.SaveAsync();

                try
                {
                    _uploadService.RemoveFileFromLocal(_utilities.FindLocalPathFromUrl(product.PhotoUrl));
                }
                catch { }

                return Redirect("/Admin/Product");
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Gallery
        public IActionResult ProductGallery()
        {
            return PartialView();
        }

        public async Task<IActionResult> Gallery(string id)
        {
            var product = await _db.ProductRepository.GetAsync(id);

            if (product != null)
            {
                var images = await _db.ProductImageRepository.GetAsync(p => p.ProductId == id, null, "Product");
                var viewModel = new ProductImageAddViewModel()
                {
                    ProductImages = images
                };

                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gallery(string id, ProductImageAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = await _db.ProductRepository.GetAsync(id);

                if (product != null)
                {
                    if (viewModel.File.IsImage())
                    {
                        PersianCalendar pc = new PersianCalendar();
                        var uploadResult = await _uploadService.UploadFile(viewModel.File, string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Request.PathBase.Value), "Products\\" + pc.GetYear(DateTime.Now) + "\\" + pc.GetMonth(DateTime.Now) + "\\" + pc.GetDayOfMonth(DateTime.Now));

                        if (uploadResult.Status)
                        {
                            var productImage = new ProductImage()
                            {
                                PhotoUrl = uploadResult.Url,
                                ProductId = id
                            };

                            await _db.ProductImageRepository.AddAsync(productImage);
                            await _db.SaveAsync();

                            return Redirect("/Admin/Product/Gallery/" + id);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "خطا در آپلود تصویر محصول");
                            var images = await _db.ProductImageRepository.GetAsync(p => p.ProductId == id, null, "Product");
                            viewModel.ProductImages = images;
                            return View(viewModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "تصویر محصول معتبر نیست");
                        var images = await _db.ProductImageRepository.GetAsync(p => p.ProductId == id, null, "Product");
                        viewModel.ProductImages = images;
                        return View(viewModel);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                var images = await _db.ProductImageRepository.GetAsync(p => p.ProductId == id, null, "Product");
                viewModel.ProductImages = images;
                return View(viewModel);
            }
        }

        public async Task<IActionResult> DeleteImage(string id)
        {
            var image = await _db.ProductImageRepository.GetAsync(id);

            if (image != null)
            {
                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("DeleteImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImageConfirm(string id)
        {
            if (ModelState.IsValid)
            {
                var image = await _db.ProductImageRepository.GetAsync(id);
                if (image != null)
                {
                    _db.ProductImageRepository.Delete(id);
                    await _db.SaveAsync();

                    try
                    {
                        _uploadService.RemoveFileFromLocal(_utilities.FindLocalPathFromUrl(image.PhotoUrl));
                    }
                    catch { }

                    return Redirect("/Admin/Product/Gallery/" + image.ProductId);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}
