using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class CouponDetailViewModel
    {
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        public string Type { get; set; }

        [Display(Name = "تخفیف")]
        public string Value { get; set; }

        [Display(Name = "محدودیت تعداد")]
        public string HasCountLimit { get; set; }

        [Display(Name = "تعداد")]
        public string CountLimit { get; set; }

        [Display(Name = "محدودیت زمان")]
        public string HasDateLimit { get; set; }

        [Display(Name = "زمان شروع")]
        public string StartDateLimit { get; set; }

        [Display(Name = "زمان پایان")]
        public string EndDateLimit { get; set; }

        [Display(Name = "محدودیت کاربر")]
        public string HasUserLimit { get; set; }

        [Display(Name = "کاربران")]
        public ICollection<string> Users { get; set; }

        [Display(Name = "محدودیت محصول")]
        public string HasProductLimit { get; set; }

        [Display(Name = "محصولات")]
        public ICollection<string> Products { get; set; }

        [Display(Name = "محدودیت دسته بندی")]
        public string HasCategoryLimit { get; set; }

        [Display(Name = "دسته بندی ها")]
        public ICollection<string> Categories { get; set; }
    }
}
