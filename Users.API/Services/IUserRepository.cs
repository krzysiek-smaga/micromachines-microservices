using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;

namespace Users.API.Services
{
    public interface IUserRepository : IBaseRepository<User>
    {
        //decimal GetUserBalance(Guid id);
        //IList<Product> GetProducts(Guid id);
        //IList<Transaction> GetTransactionHistory(Guid id);
        //IList<Order> GetOrders(Guid id);
    }
}