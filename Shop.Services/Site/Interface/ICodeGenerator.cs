using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Interface
{
    public interface ICodeGenerator
    {
        string GenerateCouponCode();
        Task<string> GenerateCouponCodeAsync();
    }
}
