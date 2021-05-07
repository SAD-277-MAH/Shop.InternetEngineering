using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Interface
{
    public interface ICouponService
    {
        bool isValid(string couponId, string userId);
        Task<bool> isValidAsync(string couponId, string userId);

        int orderDiscount(string couponId, string orderId);
        Task<int> orderDiscountAsync(string couponId, string orderId);
    }
}
