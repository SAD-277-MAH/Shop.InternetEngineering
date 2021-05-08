using Shop.Data.Context;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork<DatabaseContext> _db;

        public CouponService(IUnitOfWork<DatabaseContext> db)
        {
            _db = db;
        }

        public bool isValid(string couponId, string userId)
        {
            var coupon = _db.CouponRepository.Get(couponId);

            if (coupon == null)
            {
                return false;
            }

            if (coupon.HasUserLimit)
            {
                var userHasCoupon = _db.CouponUserRepository.Get(c => c.CouponId == couponId && c.UserId == userId, string.Empty);
                if (userHasCoupon == null)
                {
                    return false;
                }
            }

            if (coupon.HasDateLimit)
            {
                if (DateTime.Now < coupon.StartDateLimit || DateTime.Now > coupon.EndDateLimit)
                {
                    return false;
                }
            }

            if (coupon.HasCountLimit)
            {
                int orderCountUsedCoupon = (_db.CouponOrderRepository.Get(o => o.CouponId == couponId && o.Order.Status, null, "Order")).Count();
                if (orderCountUsedCoupon >= coupon.CountLimit)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> isValidAsync(string couponId, string userId)
        {
            var coupon = await _db.CouponRepository.GetAsync(couponId);

            if (coupon == null)
            {
                return false;
            }

            if (coupon.HasUserLimit)
            {
                var userHasCoupon = await _db.CouponUserRepository.GetAsync(c => c.CouponId == couponId && c.UserId == userId, string.Empty);
                if (userHasCoupon == null)
                {
                    return false;
                }
            }

            if (coupon.HasDateLimit)
            {
                if (DateTime.Now < coupon.StartDateLimit || DateTime.Now > coupon.EndDateLimit)
                {
                    return false;
                }
            }

            if (coupon.HasCountLimit)
            {
                int orderCountUsedCoupon = (await _db.CouponOrderRepository.GetAsync(o => o.CouponId == couponId && o.Order.Status, null, "Order")).Count();
                if (orderCountUsedCoupon >= coupon.CountLimit)
                {
                    return false;
                }
            }

            return true;
        }

        public int orderDiscount(string couponId, string orderId)
        {
            var coupon = _db.CouponRepository.Get(couponId);

            if (coupon == null)
            {
                return 0;
            }

            var order = _db.OrderRepository.Get(o => o.Id == orderId, "OrderDetails");

            if (order == null)
            {
                return 0;
            }

            if (!coupon.Type)
            {
                if (!(coupon.HasProductLimit || coupon.HasCategoryLimit))
                {
                    return coupon.Value;
                }
                else
                {
                    int totalPrice = 0;
                    if (coupon.HasProductLimit)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var productHasCoupon = _db.CouponProductRepository.Get(c => c.CouponId == couponId && c.ProductId == orderDetail.ProductId, string.Empty);
                            if (productHasCoupon != null)
                            {
                                totalPrice += (orderDetail.Price * orderDetail.Count);
                            }
                        }
                    }
                    else
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            int categoryId = (_db.ProductRepository.Get(orderDetail.Id)).CategoryId;
                            var productHasCoupon = _db.CouponCategoryRepository.Get(c => c.CouponId == couponId && c.CategoryId == categoryId, string.Empty);
                            if (productHasCoupon != null)
                            {
                                totalPrice += (orderDetail.Price * orderDetail.Count);
                            }
                        }
                    }

                    if (totalPrice >= coupon.Value)
                    {
                        return coupon.Value;
                    }
                    else
                    {
                        return totalPrice;
                    }
                }
            }
            else if (!(coupon.HasProductLimit || coupon.HasCategoryLimit))
            {
                return order.OrderSum * coupon.Value / 100;
            }
            else
            {
                int totalDiscount = 0;
                if (coupon.HasProductLimit)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var productHasCoupon = _db.CouponProductRepository.Get(c => c.CouponId == couponId && c.ProductId == orderDetail.ProductId, string.Empty);
                        if (productHasCoupon != null)
                        {
                            totalDiscount += ((orderDetail.Price * orderDetail.Count) * coupon.Value / 100);
                        }
                    }
                }
                else
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        int categoryId = (_db.ProductRepository.Get(orderDetail.Id)).CategoryId;
                        var productHasCoupon = _db.CouponCategoryRepository.Get(c => c.CouponId == couponId && c.CategoryId == categoryId, string.Empty);
                        if (productHasCoupon != null)
                        {
                            totalDiscount += ((orderDetail.Price * orderDetail.Count) * coupon.Value / 100);
                        }
                    }
                }

                return totalDiscount;
            }
        }

        public async Task<int> orderDiscountAsync(string couponId, string orderId)
        {
            var coupon = await _db.CouponRepository.GetAsync(couponId);

            if (coupon == null)
            {
                return 0;
            }

            var order = await _db.OrderRepository.GetAsync(o => o.Id == orderId, "OrderDetails");

            if (order == null)
            {
                return 0;
            }

            if (!coupon.Type)
            {
                if (!(coupon.HasProductLimit || coupon.HasCategoryLimit))
                {
                    return coupon.Value;
                }
                else
                {
                    int totalPrice = 0;
                    if (coupon.HasProductLimit)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var productHasCoupon = await _db.CouponProductRepository.GetAsync(c => c.CouponId == couponId && c.ProductId == orderDetail.ProductId, string.Empty);
                            if (productHasCoupon != null)
                            {
                                totalPrice += (orderDetail.Price * orderDetail.Count);
                            }
                        }
                    }
                    else
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            int categoryId = (await _db.ProductRepository.GetAsync(orderDetail.ProductId)).CategoryId;
                            var productHasCoupon = await _db.CouponCategoryRepository.GetAsync(c => c.CouponId == couponId && c.CategoryId == categoryId, string.Empty);
                            if (productHasCoupon != null)
                            {
                                totalPrice += (orderDetail.Price * orderDetail.Count);
                            }
                        }
                    }

                    if (totalPrice >= coupon.Value)
                    {
                        return coupon.Value;
                    }
                    else
                    {
                        return totalPrice;
                    }
                }
            }
            else if (!(coupon.HasProductLimit || coupon.HasCategoryLimit))
            {
                return order.OrderSum * coupon.Value / 100;
            }
            else
            {
                int totalDiscount = 0;
                if (coupon.HasProductLimit)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var productHasCoupon = await _db.CouponProductRepository.GetAsync(c => c.CouponId == couponId && c.ProductId == orderDetail.ProductId, string.Empty);
                        if (productHasCoupon != null)
                        {
                            totalDiscount += ((orderDetail.Price * orderDetail.Count) * coupon.Value / 100);
                        }
                    }
                }
                else
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        int categoryId = (await _db.ProductRepository.GetAsync(orderDetail.ProductId)).CategoryId;
                        var productHasCoupon = await _db.CouponCategoryRepository.GetAsync(c => c.CouponId == couponId && c.CategoryId == categoryId, string.Empty);
                        if (productHasCoupon != null)
                        {
                            totalDiscount += ((orderDetail.Price * orderDetail.Count) * coupon.Value / 100);
                        }
                    }
                }

                return totalDiscount;
            }
        }
    }
}
