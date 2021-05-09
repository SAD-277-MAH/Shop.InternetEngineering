using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Data.ViewModels.Site
{
    public class ProductPageViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string BrandName { get; set; }

        public string PhotoUrl { get; set; }

        public int Price { get; set; }

        public int Discount { get; set; }

        public int Quantity { get; set; }

        public string CategoryName { get; set; }


        public List<ProductImageViewModel> ProductImages { get; set; }
    }
}
