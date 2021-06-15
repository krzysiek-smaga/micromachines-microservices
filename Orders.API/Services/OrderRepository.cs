using Common.Models;
using Microsoft.EntityFrameworkCore;
using Orders.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.API.Services
{
    public class OrderRepository : IOrderRepository
    {
        DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _context;

        public OrderRepository()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "Micromachines")
               .Options;
            _context = new ApplicationDbContext(_options);
        }

        public Order Add(Order entity)
        {
            _context.Orders.Add(entity);
            Save();
            return entity;
        }

        public void Delete(Order entity)
        {
            _context.Orders.Remove(entity);
            Save();
        }

        public Order Edit(Order entity)
        {
            _context.Orders.Update(entity);
            Save();
            return entity;
        }

        public IList<Order> GetAll()
        {
            return _context.Orders.ToList();
        }

        public Order GetSingle(Func<Order, bool> condition)
        {
            return _context.Orders.Single(condition);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
