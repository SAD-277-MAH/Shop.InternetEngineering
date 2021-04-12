using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Data.Models
{
    public class CouponProduct
    {
        public CouponProduct()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        public string CouponId { get; set; }

        [Required]
        public string ProductId { get; set; }


        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
