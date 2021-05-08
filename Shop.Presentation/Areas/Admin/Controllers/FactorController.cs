using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.ViewModels.Admin;
using Shop.Data.ViewModels.Panel;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class FactorController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public FactorController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var userFactors = await _db.FactorRepository.GetAsync(f => f.Status, f => f.OrderByDescending(o => o.HasSent).ThenByDescending(o => o.DateCreated), "Order");

            var factors = _mapper.Map<List<FactorAdminViewModel>>(userFactors);
            foreach (var factor in factors)
            {
                string userId = (await _db.OrderRepository.GetAsync(o => o.Id == factor.OrderId, string.Empty)).UserId;
                var user = await _db.UserRepository.GetAsync(u => u.Id == userId, string.Empty);
                factor.FullName = user.FullName;
                factor.Email = user.Email;
            }

            return View(factors);
        }

        public async Task<IActionResult> Details(string Id)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.Id == Id, "OrderDetails");
            if (order == null)
            {
                return NotFound();
            }
            var basket = _mapper.Map<OrderDetailsViewModel>(order);
            foreach (var item in basket.OrderDetails)
            {
                var product = await _db.ProductRepository.GetAsync(item.ProductId);
                item.Name = product.Name;
                item.PhotoUrl = product.PhotoUrl;
            }
            foreach (var item in basket.CouponOrders)
            {
                var coupon = await _db.CouponRepository.GetAsync(item.CouponId);
                item.Code = coupon.Code;
            }

            return View(basket);
        }
        #endregion

        #region PostOrder
        public IActionResult PostOrder(string Id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostOrder(string Id, PostOrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var factor = await _db.FactorRepository.GetAsync(f => f.OrderId == Id, string.Empty);
                if (factor == null)
                {
                    return NotFound();
                }

                factor.HasSent = true;
                factor.PostTrackingCode = viewModel.PostTrackingCode;
                _db.FactorRepository.Update(factor);
                await _db.SaveAsync();

                return Redirect("/Admin/Factor");
            }

            return View(viewModel);
        }
        #endregion
    }
}
