using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Data.Models
{
    public class Order : BaseEntity<string>
    {
        public Order()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [Display(Name = "آدرس تحویل سفارش")]
        public string Address { get; set; }

        [Display(Name = "جمع فاکتور")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int OrderSum { get; set; }

        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Discount { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool Status { get; set; }

        [Required]
        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }


        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual ICollection<CouponOrder> CouponOrders { get; set; }
    }
}
