using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.Data.ViewModels.Admin
{
    public class UserFullDetailsViewModel
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; }

        [Display(Name = "نقش")]
        public string RoleName { get; set; }

        [Display(Name = "فغال / غیر فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "وضعیت ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string PhotoUrl { get; set; }

        [Display(Name = "تاریخ عضویت")]
        public string RegisterDate { get; set; }
    }
}
