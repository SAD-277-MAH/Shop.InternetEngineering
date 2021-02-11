using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Account
{
    public class SendEmailViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} را به صورت صحیح وارد کنید")]
        public string Email { get; set; }
    }
}
