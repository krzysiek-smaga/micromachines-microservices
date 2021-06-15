using Common.Models;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.API.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public CategoryRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Category Add(Category entity)
        {
            _context.Categories.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Category entity)
        {
            _context.Categories.Remove(entity);
            Save();
        }

        public Category Edit(Category entity)
        {
            _context.Categories.Update(entity);
            Save();
            return entity;
        }

        public IList<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public Category GetSingle(Func<Category, bool> condition)
        {
            return _context.Categories.Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
