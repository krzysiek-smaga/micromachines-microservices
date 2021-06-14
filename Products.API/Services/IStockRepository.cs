using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.API.Services
{
    public interface IStockRepository : IBaseRepository<Stock>
    {
    }
}
