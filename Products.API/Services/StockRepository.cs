using Common.Models;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.API.Services
{
    public class StockRepository : IStockRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public StockRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Stock Add(Stock entity)
        {
            _context.Stocks.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Stock entity)
        {
            _context.Stocks.Remove(entity);
            Save();
        }

        public Stock Edit(Stock entity)
        {
            _context.Stocks.Update(entity);
            Save();
            return entity;
        }

        public IList<Stock> GetAll()
        {
            return _context.Stocks.Include(x => x.Balances).ToList();
        }

        public Stock GetSingle(Func<Stock, bool> condition)
        {
            return _context.Stocks.Include(x => x.Balances).Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
