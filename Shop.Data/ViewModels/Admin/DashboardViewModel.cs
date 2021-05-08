using Shop.Data.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            Chart = new List<ChartViewModel>();
        }

        public int UnSendOrders { get; set; }
        public int UnSeenTickets { get; set; }
        public int UnApprovedComments { get; set; }

        public List<ChartViewModel> Chart { get; set; }
    }
}
