using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Panel
{
    public class OrderDetailsCouponViewModel
    {
        [Required]
        public string CouponId { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Code { get; set; }
    }
}
