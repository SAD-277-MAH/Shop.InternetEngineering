using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Account
{
    public class SetPasswordViewModel
    {
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
