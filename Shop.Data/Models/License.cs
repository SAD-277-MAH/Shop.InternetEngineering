using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.Models
{
    public class License : BaseEntity<int>
    {
        public License()
        {
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string Name { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public string Content { get; set; }
    }
}
