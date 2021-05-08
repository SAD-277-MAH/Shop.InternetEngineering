using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public PaymentController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var userFactors = await _db.FactorRepository.GetAsync(f => f.Status, f => f.OrderByDescending(o => o.HasSent).ThenByDescending(o => o.DateCreated), "Order");

            var factors = _mapper.Map<List<PaymentAdminViewModel>>(userFactors);
            foreach (var factor in factors)
            {
                string userId = (await _db.OrderRepository.GetAsync(o => o.Id == factor.OrderId, string.Empty)).UserId;
                var user = await _db.UserRepository.GetAsync(u => u.Id == userId, string.Empty);
                factor.FullName = user.FullName;
                factor.Email = user.Email;
            }

            return View(factors);
        }
        #endregion
    }
}
