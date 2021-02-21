using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Shop.Common.Extentions
{
    public static class Extentions
    {
        public static bool IsImage(this IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    var image = Image.FromStream(file.OpenReadStream());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
