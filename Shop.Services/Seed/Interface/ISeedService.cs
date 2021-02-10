using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Services.Seed.Interface
{
    public interface ISeedService
    {
        void SeedRoles();
        void SeedUsers();
    }
}
