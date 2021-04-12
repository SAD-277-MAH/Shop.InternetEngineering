using Shop.Data.Context;
using Shop.Repo.Infrastructure;
using Shop.Services.Site.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Service
{
    public class CodeGenerator : ICodeGenerator
    {
        private readonly IUnitOfWork<DatabaseContext> _db;

        public CodeGenerator(IUnitOfWork<DatabaseContext> db)
        {
            _db = db;
        }

        public string GenerateCouponCode()
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            do
            {
                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
            } while (_db.CouponRepository.CodeExists(str_build.ToString()));

            return str_build.ToString();
        }

        public async Task<string> GenerateCouponCodeAsync()
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            do
            {
                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
            } while (await _db.CouponRepository.CodeExistsAsync(str_build.ToString()));

            return str_build.ToString();
        }
    }
}
