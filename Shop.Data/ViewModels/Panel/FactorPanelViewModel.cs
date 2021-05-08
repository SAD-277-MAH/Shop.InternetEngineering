using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Panel
{
    public class FactorPanelViewModel
    {
        public string OrderId { get; set; }

        [Display(Name = "تاریخ خرید")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string DateCreated { get; set; }

        [Display(Name = "مبلغ پرداختی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Price { get; set; }

        [Display(Name = "کد رهگیری بانک")]
        public string BankTrackingCode { get; set; }

        [Display(Name = "وضعیت ارسال")]
        public bool HasSent { get; set; }

        [Display(Name = "کد رهگیری پست")]
        public string PostTrackingCode { get; set; }
    }
}
