using Shop.Common.ReturnMessage;
using Shop.Data.Context;
using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly ICouponService _couponService;

        public OrderService(IUnitOfWork<DatabaseContext> db, ICouponService couponService)
        {
            _db = db;
            _couponService = couponService;
        }

        public Response AddCouponToOrder(string userId, string couponCode)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            string couponId = string.Empty;
            couponId = (_db.CouponRepository.Get(c => c.Code == couponCode, string.Empty))?.Id;

            if (order == null || order.Status)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            if (!_couponService.isValid(couponId, userId))
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var couponOrder = new CouponOrder()
            {
                OrderId = order.Id,
                CouponId = couponId
            };

            _db.CouponOrderRepository.Add(couponOrder);
            _db.Save();

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> AddCouponToOrderAsync(string userId, string couponCode)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);
            string couponId = string.Empty;
            couponId = (await _db.CouponRepository.GetAsync(c => c.Code == couponCode, string.Empty))?.Id;

            if (order == null || order.Status)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            if (!await _couponService.isValidAsync(couponId, userId))
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var couponOrder = new CouponOrder()
            {
                OrderId = order.Id,
                CouponId = couponId
            };

            await _db.CouponOrderRepository.AddAsync(couponOrder);
            await _db.SaveAsync();

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response AddToOrder(string userId, string productId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var product = _db.ProductRepository.Get(productId);

            if (product != null)
            {
                if (order == null)
                {
                    order = new Order()
                    {
                        OrderSum = (product.Price - product.Discount),
                        Discount = 0,
                        Status = false,
                        UserId = userId
                    };
                    _db.OrderRepository.Add(order);
                    _db.Save();
                }

                var orderDetail = _db.OrderDetailRepository.Get(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

                if (orderDetail == null)
                {
                    if (product.Quantity >= 1)
                    {
                        orderDetail = new OrderDetail()
                        {
                            Count = 1,
                            Price = (product.Price - product.Discount),
                            OrderId = order.Id,
                            ProductId = productId
                        };
                        _db.OrderDetailRepository.Add(orderDetail);
                        _db.Save();
                    }
                    else
                    {
                        return new Response(false, "موجودی کالا کافی نیست");
                    }
                }
                else
                {
                    if (product.Quantity >= orderDetail.Count + 1)
                    {
                        orderDetail.Count += 1;
                        _db.OrderDetailRepository.Update(orderDetail);
                        _db.Save();
                    }
                    else
                    {
                        return new Response(false, "موجودی کالا کافی نیست");
                    }
                }
            }
            else
            {
                return new Response(false, "کالا یافت نشد");
            }

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> AddToOrderAsync(string userId, string productId)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);
            var product = await _db.ProductRepository.GetAsync(productId);

            if (product != null)
            {
                if (order == null)
                {
                    order = new Order()
                    {
                        OrderSum = (product.Price - product.Discount),
                        Discount = 0,
                        Status = false,
                        UserId = userId
                    };
                    await _db.OrderRepository.AddAsync(order);
                    await _db.SaveAsync();
                }

                var orderDetail = await _db.OrderDetailRepository.GetAsync(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

                if (orderDetail == null)
                {
                    if (product.Quantity >= 1)
                    {
                        orderDetail = new OrderDetail()
                        {
                            Count = 1,
                            Price = (product.Price - product.Discount),
                            OrderId = order.Id,
                            ProductId = productId
                        };
                        await _db.OrderDetailRepository.AddAsync(orderDetail);
                        await _db.SaveAsync();
                    }
                    else
                    {
                        return new Response(false, "موجودی کالا کافی نیست");
                    }
                }
                else
                {
                    if (product.Quantity >= orderDetail.Count + 1)
                    {
                        orderDetail.Count += 1;
                        _db.OrderDetailRepository.Update(orderDetail);
                        await _db.SaveAsync();
                    }
                    else
                    {
                        return new Response(false, "موجودی کالا کافی نیست");
                    }
                }
            }
            else
            {
                return new Response(false, "کالا یافت نشد");
            }

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response DecreaseCount(string userId, string productId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var product = _db.ProductRepository.Get(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = _db.OrderDetailRepository.Get(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            if (orderDetail.Count == 1)
            {
                _db.OrderDetailRepository.Delete(orderDetail);
                _db.Save();
            }
            else
            {
                orderDetail.Count -= 1;
                _db.OrderDetailRepository.Update(orderDetail);
                _db.Save();
            }

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> DecreaseCountAsync(string userId, string productId)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);
            var product = await _db.ProductRepository.GetAsync(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = await _db.OrderDetailRepository.GetAsync(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            if (orderDetail.Count == 1)
            {
                _db.OrderDetailRepository.Delete(orderDetail);
                await _db.SaveAsync();
            }
            else
            {
                orderDetail.Count -= 1;
                _db.OrderDetailRepository.Update(orderDetail);
                await _db.SaveAsync();
            }

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response IncreaseCount(string userId, string productId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var product = _db.ProductRepository.Get(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = _db.OrderDetailRepository.Get(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            if (product.Quantity >= orderDetail.Count + 1)
            {
                orderDetail.Count += 1;
                _db.OrderDetailRepository.Update(orderDetail);
                _db.Save();
            }
            else
            {
                return new Response(false, "موجودی کالا کافی نیست");
            }

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> IncreaseCountAsync(string userId, string productId)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);
            var product = await _db.ProductRepository.GetAsync(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = await _db.OrderDetailRepository.GetAsync(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            if (product.Quantity >= orderDetail.Count + 1)
            {
                orderDetail.Count += 1;
                _db.OrderDetailRepository.Update(orderDetail);
                await _db.SaveAsync();
            }
            else
            {
                return new Response(false, "موجودی کالا کافی نیست");
            }

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response RemoveCouponFromOrder(string userId, string couponId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var couponOrder = _db.CouponOrderRepository.Get(c => c.OrderId == order.Id && c.CouponId == couponId, string.Empty);

            if (couponOrder == null)
            {
                return new Response(false, "کد تخفیف در سبد خرید یافت نشد");
            }

            _db.CouponOrderRepository.Delete(couponOrder);
            _db.Save();

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> RemoveCouponFromOrderAsync(string userId, string couponId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var couponOrder = await _db.CouponOrderRepository.GetAsync(c => c.OrderId == order.Id && c.CouponId == couponId, string.Empty);

            if (couponOrder == null)
            {
                return new Response(false, "کد تخفیف در سبد خرید یافت نشد");
            }

            _db.CouponOrderRepository.Delete(couponOrder);
            await _db.SaveAsync();

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response RemoveFromOrder(string userId, string productId)
        {
            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);
            var product = _db.ProductRepository.Get(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = _db.OrderDetailRepository.Get(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            _db.OrderDetailRepository.Delete(orderDetail);
            _db.Save();

            UpdateOrder(userId);
            return new Response(true, string.Empty);
        }

        public async Task<Response> RemoveFromOrderAsync(string userId, string productId)
        {
            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);
            var product = await _db.ProductRepository.GetAsync(productId);

            if (product == null)
            {
                return new Response(false, "کالا یافت نشد");
            }

            if (order == null)
            {
                return new Response(false, "سبد خرید یافت نشد");
            }

            var orderDetail = await _db.OrderDetailRepository.GetAsync(o => o.OrderId == order.Id && o.ProductId == productId, string.Empty);

            if (orderDetail == null)
            {
                return new Response(false, "کالا در سبد خرید یافت نشد");
            }

            _db.OrderDetailRepository.Delete(orderDetail);
            await _db.SaveAsync();

            await UpdateOrderAsync(userId);
            return new Response(true, string.Empty);
        }

        public Response UpdateOrder(string userId)
        {
            bool isChanged = false;

            var order = _db.OrderRepository.Get(o => o.UserId == userId && !o.Status, string.Empty);

            if (order == null)
            {
                return new Response(false, string.Empty);
            }

            var orderDetails = _db.OrderDetailRepository.Get(o => o.OrderId == order.Id, null, string.Empty);
            int orderSum = 0;
            foreach (var orderDetail in orderDetails)
            {
                var product = _db.ProductRepository.Get(orderDetail.ProductId);

                if (product == null || product.Quantity == 0)
                {
                    _db.OrderDetailRepository.Delete(orderDetail.Id);
                    _db.Save();
                    isChanged = true;
                }

                if (product.Quantity < orderDetail.Count)
                {
                    orderDetail.Count = product.Quantity;
                    _db.OrderDetailRepository.Update(orderDetail);
                    _db.Save();
                    isChanged = true;
                }

                if ((product.Price - product.Discount) != orderDetail.Price)
                {
                    orderDetail.Price = (product.Price - product.Discount);
                    _db.OrderDetailRepository.Update(orderDetail);
                    _db.Save();
                    isChanged = true;
                }

                orderSum += orderDetail.Price * orderDetail.Count;
            }
            if (order.OrderSum != orderSum)
            {
                order.OrderSum = orderSum;
                _db.OrderRepository.Update(order);
                _db.Save();
                isChanged = true;
            }

            var coupons = _db.CouponOrderRepository.Get(c => c.OrderId == order.Id, null, string.Empty);
            int totalDiscount = 0;
            foreach (var coupon in coupons)
            {
                int discount = _couponService.orderDiscount(coupon.CouponId, order.Id);
                if (!_couponService.isValid(coupon.CouponId, userId) || discount == 0)
                {
                    _db.CouponOrderRepository.Delete(coupon.Id);
                    _db.Save();
                    isChanged = true;
                }
                totalDiscount += discount;
            }
            if (order.Discount != totalDiscount)
            {
                order.Discount = totalDiscount;
                _db.OrderRepository.Update(order);
                _db.Save();
                isChanged = true;
            }

            return new Response(isChanged, string.Empty);
        }

        public async Task<Response> UpdateOrderAsync(string userId)
        {
            bool isChanged = false;

            var order = await _db.OrderRepository.GetAsync(o => o.UserId == userId && !o.Status, string.Empty);

            if (order == null)
            {
                return new Response(false, string.Empty);
            }

            var orderDetails = await _db.OrderDetailRepository.GetAsync(o => o.OrderId == order.Id, null, string.Empty);
            int orderSum = 0;
            foreach (var orderDetail in orderDetails)
            {
                var product = await _db.ProductRepository.GetAsync(orderDetail.ProductId);

                if (product == null || product.Quantity == 0)
                {
                    _db.OrderDetailRepository.Delete(orderDetail.Id);
                    await _db.SaveAsync();
                    isChanged = true;
                }

                if (product.Quantity < orderDetail.Count)
                {
                    orderDetail.Count = product.Quantity;
                    _db.OrderDetailRepository.Update(orderDetail);
                    await _db.SaveAsync();
                    isChanged = true;
                }

                if ((product.Price - product.Discount) != orderDetail.Price)
                {
                    orderDetail.Price = (product.Price - product.Discount);
                    _db.OrderDetailRepository.Update(orderDetail);
                    await _db.SaveAsync();
                    isChanged = true;
                }

                orderSum += orderDetail.Price * orderDetail.Count;
            }
            if (order.OrderSum != orderSum)
            {
                order.OrderSum = orderSum;
                _db.OrderRepository.Update(order);
                await _db.SaveAsync();
                isChanged = true;
            }

            var coupons = await _db.CouponOrderRepository.GetAsync(c => c.OrderId == order.Id, null, string.Empty);
            int totalDiscount = 0;
            foreach (var coupon in coupons)
            {
                int discount = await _couponService.orderDiscountAsync(coupon.CouponId, order.Id);
                if (!_couponService.isValid(coupon.CouponId, userId) || discount == 0)
                {
                    _db.CouponOrderRepository.Delete(coupon.Id);
                    await _db.SaveAsync();
                    isChanged = true;
                }
                totalDiscount += discount;
            }
            if (order.Discount != totalDiscount)
            {
                order.Discount = totalDiscount;
                _db.OrderRepository.Update(order);
                await _db.SaveAsync();
                isChanged = true;
            }

            return new Response(isChanged, string.Empty);
        }
    }
}
