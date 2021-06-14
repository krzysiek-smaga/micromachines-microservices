using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transactions.API.Data;

namespace Transactions.API.Services
{
    public class TransactionRepository : ITransactionRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public TransactionRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Transaction Add(Transaction entity)
        {
            _context.Transactions.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Transaction entity)
        {
            _context.Transactions.Remove(entity);
            Save();
        }

        public Transaction Edit(Transaction entity)
        {
            _context.Transactions.Update(entity);
            Save();
            return entity;
        }

        public IList<Transaction> GetAll()
        {
            return _context.Transactions.AsNoTracking().ToList();
        }

        public Transaction GetSingle(Func<Transaction, bool> condition)
        {
            return _context.Transactions.Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
