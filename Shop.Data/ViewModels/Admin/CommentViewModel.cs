using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class CommentViewModel
    {
        public string Id { get; set; }

        [Display(Name = "تاریخ ارسال")]
        public string Date { get; set; }

        [Display(Name = "متن پیام")]
        public string Text { get; set; }

        [Display(Name = "وضعیت")]
        public int Status { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "نام محصول")]
        public string ProductName { get; set; }
    }
}
