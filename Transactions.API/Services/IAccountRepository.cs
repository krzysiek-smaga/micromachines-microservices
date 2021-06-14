using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transactions.API.Services
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
    }
}
