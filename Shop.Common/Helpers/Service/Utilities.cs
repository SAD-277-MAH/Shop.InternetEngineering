using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shop.Common.Helpers.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Common.Helpers.Service
{
    public class Utilities : IUtilities
    {
        public string FindLocalPathFromUrl(string url)
        {
            var temp = url.Replace("https://", "").Replace("http://", "").Split('/').Skip(2);

            return temp.Aggregate("", (current, item) => current + item + "\\").TrimEnd('\\');
        }
    }
}
