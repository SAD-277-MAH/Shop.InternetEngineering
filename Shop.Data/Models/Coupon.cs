using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.Models
{
    public class Coupon : BaseEntity<string>
    {
        public Coupon()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }


        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool Type { get; set; }

        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Value { get; set; }

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

        [Display(Name = "محدودیت محصول")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasProductLimit { get; set; }

        [Display(Name = "محدودیت دسته بندی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool HasCategoryLimit { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Code { get; set; }


        public virtual ICollection<CouponUser> CouponUsers { get; set; }

        public virtual ICollection<CouponProduct> CouponProducts { get; set; }

        public virtual ICollection<CouponCategory> CouponCategories { get; set; }

        public virtual ICollection<CouponOrder> CouponOrders { get; set; }
    }
}
