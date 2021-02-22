using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.Models
{
    public class ProductImage : BaseEntity<string>
    {
        public ProductImage()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [Display(Name = "تصویر محصول")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "مقدار {0} نمی تواند بیشتر از {1} باشد")]
        public string PhotoUrl { get; set; }

        [Required]
        public string ProductId { get; set; }


        public virtual Product Product { get; set; }
    }
}
