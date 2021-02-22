using Microsoft.AspNetCore.Http;
using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class ProductImageAddViewModel
    {
        [Display(Name = "تصویر محصول")]
        [Required(ErrorMessage = "{0} را انتخاب کنید")]
        public IFormFile File { get; set; }

        public IEnumerable<ProductImage> ProductImages { get; set; }
    }
}
