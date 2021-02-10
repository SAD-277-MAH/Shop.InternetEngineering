using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} را به صورت صحیح وارد کنید")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
