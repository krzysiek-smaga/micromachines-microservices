using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Users.API.Data;

namespace Users.API.Services
{
    public class UserRepository : IUserRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public UserRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public User Add(User entity)
        {
            _context.Users.Add(entity);
            Save();
            return entity;
        }

        public void Delete(User entity)
        {
            _context.Users.Remove(entity);
            Save();
        }

        public User Edit(User entity)
        {
            _context.Users.Update(entity);
            Save();
            return entity;
        }

        public IList<User> GetAll()
        {
            return _context.Users.Include(x => x.Basket).AsNoTracking().ToList();
        }

        //public IList<Order> GetOrders(Guid id)
        //{
        //    return _context.Orders.Include(x => x.Basket).Where(x => x.PurchaseByUserID == id).ToList();
        //}

        //public IList<Product> GetProducts(Guid id)
        //{
        //    var products = new List<Product>();
        //    var userBasket = _context.Users.Single(x => x.ID == id).Basket;

        //    foreach (var productQuantity in userBasket)
        //    {
        //        products.Add(_context.Products.Single(x => x.ID == productQuantity.ProductID));
        //    }

        //    return products;
        //}

        public User GetSingle(Func<User, bool> condition)
        {
            return _context.Users.Include(x => x.Basket).Single(condition);
        }

        //public IList<Transaction> GetTransactionHistory(Guid id)
        //{
        //    var userTransactions = new List<Transaction>();
        //    var userAccounts = _context.Accounts.Where(x => x.UserID == id).ToList();

        //    foreach (var account in userAccounts)
        //    {
        //        userTransactions.AddRange(_context.Transactions.Where(x => x.AccountFromID == account.ID || x.AccountToID == account.ID).ToList());
        //    }

        //    return userTransactions;
        //}

        //public decimal GetUserBalance(Guid id)
        //{
        //    return _context.Accounts.Where(x => x.UserID == id).Sum(x => x.Balance);
        //}

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
