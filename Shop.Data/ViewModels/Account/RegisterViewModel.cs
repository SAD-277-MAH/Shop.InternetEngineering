using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        [EmailAddress(ErrorMessage = "{0} را به صورت صحیح وارد کنید")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تأیید کلمه عبور")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "تأیید کلمه عبور صحیح نمی باشد")]
        public string ConfirmPassword { get; set; }
    }
}
