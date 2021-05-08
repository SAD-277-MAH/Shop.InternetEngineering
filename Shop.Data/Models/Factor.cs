using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Data.Models
{
    public class Factor : BaseEntity<string>
    {
        public Factor()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }


        [Display(Name = "وضعیت پرداخت")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public bool Status { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "مقدار {0} را وارد کنید")]
        public int Price { get; set; }

        [Display(Name = "کد رهگیری بانک")]
        public string BankTrackingCode { get; set; }

        [Display(Name = "وضعیت ارسال")]
        public bool HasSent { get; set; }

        [Display(Name = "کد رهگیری پست")]
        public string PostTrackingCode { get; set; }

        [Required]
        public string OrderId { get; set; }


        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
