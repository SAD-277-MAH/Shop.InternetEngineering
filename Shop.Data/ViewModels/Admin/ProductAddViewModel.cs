using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class ProductAddViewModel
    {
        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "برند")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string BrandName { get; set; }

        [Display(Name = "تصویر محصول")]
        [Required(ErrorMessage = "{0} را انتخاب کنید")]
        public IFormFile File { get; set; }

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Price { get; set; } = 0;

        [Display(Name = "تخفیف")]
        public int Discount { get; set; } = 0;

        [Display(Name = "تعداد")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Quantity { get; set; } = 0;

        [Required]
        public int CategoryId { get; set; }
    }
}
