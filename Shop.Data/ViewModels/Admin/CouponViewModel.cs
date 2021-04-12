using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class CouponViewModel
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string Type { get; set; }

        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int PercentDiscount { get; set; }

        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int ValueDiscount { get; set; }

        [Display(Name = "محدودیت تعداد")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasCountLimit { get; set; }

        [Display(Name = "تعداد")]
        public int CountLimit { get; set; }

        [Display(Name = "محدودیت زمان")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasDateLimit { get; set; }

        [Display(Name = "زمان شروع")]
        public DateTime StartDateLimit { get; set; }

        [Display(Name = "زمان پایان")]
        public DateTime EndDateLimit { get; set; }

        [Display(Name = "محدودیت کاربر")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasUserLimit { get; set; }

        [Display(Name = "کاربران")]
        public ICollection<string> Users { get; set; }

        [Display(Name = "محدودیت محصول / دسته بندی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasProductOrCategoryLimit { get; set; }

        [Display(Name = "نوع محدودیت محصول یا دسته بندی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string ProductOrCategoryLimit { get; set; }

        [Display(Name = "محصولات")]
        public ICollection<string> Products { get; set; }

        [Display(Name = "دسته بندی ها")]
        public ICollection<int> Categories { get; set; }
    }
}
