using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class LicenseViewModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string Content { get; set; }
    }
}
