using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Panel
{
    public class OrderDetailsDetailViewModel
    {
        [Required]
        public string ProductId { get; set; }

        [Display(Name = "تعداد")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Count { get; set; }

        [Display(Name = "قیمت واحد")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Price { get; set; }

        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "تصویر محصول")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string PhotoUrl { get; set; }
    }
}
