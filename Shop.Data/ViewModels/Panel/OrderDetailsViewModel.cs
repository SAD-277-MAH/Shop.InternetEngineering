using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Panel
{
    public class OrderDetailsViewModel
    {
        [Display(Name = "تاریخ ایجاد")]
        public string DateCreated { get; set; }

        [Display(Name = "جمع فاکتور")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int OrderSum { get; set; }

        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Discount { get; set; }

        public List<OrderDetailsDetailViewModel> OrderDetails { get; set; }

        public List<OrderDetailsCouponViewModel> CouponOrders { get; set; }
    }
}
