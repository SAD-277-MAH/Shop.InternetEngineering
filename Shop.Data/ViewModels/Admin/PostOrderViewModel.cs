using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class PostOrderViewModel
    {
        [Display(Name = "کد رهگیری پست")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string PostTrackingCode { get; set; }
    }
}
