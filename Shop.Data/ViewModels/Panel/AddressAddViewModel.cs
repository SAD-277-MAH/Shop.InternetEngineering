using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Panel
{
    public class AddressAddViewModel
    {
        [Display(Name = "استان")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Province { get; set; }

        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string City { get; set; }

        [Display(Name = "نشانی پستی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string PostalAddress { get; set; }

        [Display(Name = "پلاک")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string NO { get; set; }

        [Display(Name = "واحد")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Unit { get; set; }

        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(20, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string PostalCode { get; set; }
    }
}
