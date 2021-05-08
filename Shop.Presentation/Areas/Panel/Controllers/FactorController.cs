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
    public class FactorController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public FactorController(IUnitOfWork<DatabaseContext> db, IMapper mapper, UserManager<User> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;

            var userFactors = await _db.FactorRepository.GetAsync(f => f.Order.UserId == userId && f.Status, f => f.OrderByDescending(o => o.DateCreated), "Order");

            var factors = _mapper.Map<List<FactorPanelViewModel>>(userFactors);

            return View(factors);
        }

        public async Task<IActionResult> Details(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var order = await _db.OrderRepository.GetAsync(o => o.Id == Id && o.UserId == userId, "OrderDetails");
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
    }
}
