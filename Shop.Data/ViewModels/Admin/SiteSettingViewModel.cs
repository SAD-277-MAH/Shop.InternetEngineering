using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class SiteSettingViewModel
    {
        [Display(Name = "نام فروشگاه")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string ShopName { get; set; }

        [Display(Name = "توضیحات مختصر")]
        [DataType(DataType.MultilineText)]
        public string ShopDesc { get; set; }

        [Display(Name = "کلمات کلیدی")]
        [DataType(DataType.MultilineText)]
        public string ShopKeyWords { get; set; }
    }
}
