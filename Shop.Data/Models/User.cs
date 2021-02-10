using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string FullName { get; set; }

        [Display(Name = "تصویر پروفایل")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string PhotoUrl { get; set; }

        [Display(Name = "فعال / غیر فعال")]
        [Required]
        public bool IsActive { get; set; }
    }
}
