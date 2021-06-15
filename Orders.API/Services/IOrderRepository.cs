using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.API.Services
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}
