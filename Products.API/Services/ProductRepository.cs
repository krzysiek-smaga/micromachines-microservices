using Common.Models;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecool.MicroMachines.Services.Services
{
    public class ProductRepository : IProductRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public ProductRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Product Add(Product entity)
        {
            _context.Products.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Product entity)
        {
            _context.Products.Remove(entity);
            Save();
        }

        public Product Edit(Product entity)
        {
            _context.Products.Update(entity);
            Save();
            return entity;
        }

        public IList<Product> GetAll()
        {
            return _context.Products.AsNoTracking().ToList();
        }

        public Product GetSingle(Func<Product, bool> condition)
        {
            return _context.Products.Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
