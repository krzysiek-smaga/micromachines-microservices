using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecool.MicroMachines.Services.Services
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}
