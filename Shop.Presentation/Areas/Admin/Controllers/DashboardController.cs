using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.Extentions;
using Shop.Data.Context;
using Shop.Data.ViewModels.Admin;
using Shop.Data.ViewModels.Common;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;

        public DashboardController(IUnitOfWork<DatabaseContext> db)
        {
            _db = db;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();
            var factors = await _db.FactorRepository.GetAsync();

            viewModel.UnSendOrders = factors.Where(f => f.Status && !f.HasSent).Count();
            //TODO -------------------
            viewModel.UnSeenTickets = 10;
            viewModel.UnApprovedComments = (await _db.CommentRepository.GetAsync(c => c.Status == 0, null, string.Empty)).Count();

            DateTime today = DateTime.Now;
            for (int i = 6; i >= 0; i--)
            {
                DateTime date = today.AddDays(-i);
                var factorsOfDay = factors.Where(f => f.Status && f.DateCreated.Year == date.Year && f.DateCreated.Month == date.Month && f.DateCreated.Day == date.Day);
                int sumOfDay = factorsOfDay.Sum(f => f.Price);
                var chartData = new ChartViewModel()
                {
                    Label = date.ToShamsiDate(),
                    Value = sumOfDay
                };
                viewModel.Chart.Add(chartData);
            }

            return View(viewModel);
        }
        #endregion
    }
}
