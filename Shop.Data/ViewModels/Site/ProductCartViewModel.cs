using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Data.ViewModels.Site
{
    public class ProductCartViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string PhotoUrl { get; set; }

        public int Price { get; set; }

        public int Discount { get; set; }
    }
}
