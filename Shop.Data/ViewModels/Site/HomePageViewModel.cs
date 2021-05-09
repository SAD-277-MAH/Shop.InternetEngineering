using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Data.ViewModels.Site
{
    public class HomePageViewModel
    {
        public List<SiteCategoryViewModel> Categories { get; set; }

        public List<ProductCartViewModel> MostDiscountProducts { get; set; }

        public List<ProductCartViewModel> MostPurchasedProducts { get; set; }

        public List<ProductCartViewModel> MostRecentProducts { get; set; }
    }
}
