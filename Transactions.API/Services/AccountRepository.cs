using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transactions.API.Data;

namespace Transactions.API.Services
{
    public class AccountRepository : IAccountRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public AccountRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Account Add(Account entity)
        {
            _context.Accounts.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Account entity)
        {
            _context.Accounts.Remove(entity);
            Save();
        }

        public Account Edit(Account entity)
        {
            _context.Accounts.Update(entity);
            Save();
            return entity;
        }

        public IList<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account GetSingle(Func<Account, bool> condition)
        {
            return _context.Accounts.Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
