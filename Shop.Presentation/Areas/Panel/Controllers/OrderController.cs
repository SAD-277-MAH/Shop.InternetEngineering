using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Data.ViewModels.Panel;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Panel.Controllers
{
    [Area("Panel")]
    [Authorize(Policy = "RequireUserRole")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IOrderService _orderService;

        public OrderController(IUnitOfWork<DatabaseContext> db, IMapper mapper, UserManager<User> userManager, IOrderService orderService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _orderService = orderService;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, "OrderDetails,CouponOrders");
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

            var userAddresses = await _db.AddressRepository.GetAsync(a => a.UserId == userId, o => o.OrderBy(a => a.DateCreated), string.Empty);
            var Addresses = _mapper.Map<List<AddressSelectListViewModel>>(userAddresses);
            ViewData["Addresses"] = new SelectList(Addresses, "Address", "Address");
            return View(basket);
        }
        #endregion

        #region AddToOrder
        public async Task<IActionResult> AddToOrder(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _orderService.AddToOrderAsync(userId, Id);

            return Redirect("/Panel/Order");
        }
        #endregion

        #region RemoveFromOrder
        public async Task<IActionResult> RemoveFromOrder(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _orderService.RemoveFromOrderAsync(userId, Id);

            return Redirect("/Panel/Order");
        }
        #endregion

        #region IncreaseCount
        public async Task<IActionResult> IncreaseCount(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _orderService.IncreaseCountAsync(userId, Id);

            return Redirect("/Panel/Order");
        }
        #endregion

        #region DecreaseCount
        public async Task<IActionResult> DecreaseCount(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _orderService.DecreaseCountAsync(userId, Id);

            return Redirect("/Panel/Order");
        }
        #endregion

        #region AddCouponToOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCouponToOrder(string couponCode)
        {
            if (ModelState.IsValid)
            {
                string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                var res = await _orderService.AddCouponToOrderAsync(userId, couponCode);

                if (!res.Status)
                {
                    ModelState.AddModelError("", res.Message);
                }
            }
            return Redirect("/Panel/Order");
        }
        #endregion

        #region RemoveCouponFromOrder
        public async Task<IActionResult> RemoveCouponFromOrder(string Id)
        {
            string userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _orderService.RemoveCouponFromOrderAsync(userId, Id);

            return Redirect("/Panel/Order");
        }
        #endregion
    }
}
