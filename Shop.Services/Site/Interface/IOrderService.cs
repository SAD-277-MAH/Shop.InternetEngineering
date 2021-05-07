using Shop.Common.ReturnMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Interface
{
    public interface IOrderService
    {
        Response AddToOrder(string userId, string productId);
        Task<Response> AddToOrderAsync(string userId, string productId);

        Response RemoveFromOrder(string userId, string productId);
        Task<Response> RemoveFromOrderAsync(string userId, string productId);

        Response IncreaseCount(string userId, string productId);
        Task<Response> IncreaseCountAsync(string userId, string productId);

        Response DecreaseCount(string userId, string productId);
        Task<Response> DecreaseCountAsync(string userId, string productId);

        Response AddCouponToOrder(string userId, string couponCode);
        Task<Response> AddCouponToOrderAsync(string userId, string couponCode);

        Response RemoveCouponFromOrder(string userId, string couponId);
        Task<Response> RemoveCouponFromOrderAsync(string userId, string couponId);

        Response UpdateOrder(string userId);
        Task<Response> UpdateOrderAsync(string userId);
    }
}
