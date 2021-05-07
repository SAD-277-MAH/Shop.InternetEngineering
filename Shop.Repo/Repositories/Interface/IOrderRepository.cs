using Shop.Data.Models;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repo.Repositories.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}
