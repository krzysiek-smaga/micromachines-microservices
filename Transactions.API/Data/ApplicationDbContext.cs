using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transactions.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            if (!Transactions.Any())
            {
                //var users = new User[]
                //{
                //    new User { ID = Guid.Parse("00000000-0000-0000-0000-000000000001"), FirstName = "Krzysiek", LastName = "Smaga", Address = "Porabka 69 34-642 Dobra",
                //        Email = "krzysiek.at@gmail.com", PhoneNo = "788804047"},
                //    new User { ID = Guid.Parse("00000000-0000-0000-0000-000000000002"), FirstName = "Dominik", LastName = "Starzyk", Address = "Nowa Huta 1 Krakow",
                //        Email = "dominik.st@gmail.com", PhoneNo = "123456789"}
                //};
                //Users.AddRange(users);
                //SaveChanges();

                var accounts = new Account[]
                {
                    new Account { ID = Guid.Parse("00000000-0000-0000-0000-000000000000"), Name = "StoreAccount", Balance = 0.00M, IsClosed = false },
                    new Account { ID = Guid.Parse("00000000-0000-0000-0001-000000000000"), Name = "Alior", Balance = 1000.00M, UserID = Guid.Parse("00000000-0000-0000-0000-000000000001"), IsClosed = false },
                    new Account { ID = Guid.Parse("00000000-0000-0000-0002-000000000000"), Name = "MBank", Balance = 50.00M, UserID = Guid.Parse("00000000-0000-0000-0000-000000000001"), IsClosed = false },
                    new Account { ID = Guid.Parse("00000000-0000-0000-0003-000000000000"), Name = "Alior", Balance = 200.00M, UserID = Guid.Parse("00000000-0000-0000-0000-000000000002"), IsClosed = false },
                    new Account { ID = Guid.Parse("00000000-0000-0000-0004-000000000000"), Name = "PKO", Balance = 20.00M, UserID = Guid.Parse("00000000-0000-0000-0000-000000000002"), IsClosed = false }
                };
                Accounts.AddRange(accounts);
                SaveChanges();

                //var categories = new Category[]
                //{
                //    new Category { ID = Guid.Parse("00000000-0000-0001-0000-000000000000"), Name = "Car Models"},
                //    new Category { ID = Guid.Parse("00000000-0000-0002-0000-000000000000"), Name = "Plane Models"}
                //};
                //Categories.AddRange(categories);
                //SaveChanges();

                //var products = new Product[]
                //{
                //    new Product{ ID = Guid.Parse("00000000-0001-0000-0000-000000000000"), Name = "BMW X5", Price = 100.00M, CategoryID = Guid.Parse("00000000-0000-0001-0000-000000000000") },
                //    new Product{ ID = Guid.Parse("00000000-0002-0000-0000-000000000000"), Name = "Ferrari Enzo", Price = 200.00M, CategoryID = Guid.Parse("00000000-0000-0001-0000-000000000000") },
                //    new Product{ ID = Guid.Parse("00000000-0003-0000-0000-000000000000"), Name = "Boeing 575", Price = 100.00M, CategoryID = Guid.Parse("00000000-0000-0002-0000-000000000000") },
                //    new Product{ ID = Guid.Parse("00000000-0004-0000-0000-000000000000"), Name = "F16", Price = 150.00M, CategoryID = Guid.Parse("00000000-0000-0002-0000-000000000000") }
                //};
                //Products.AddRange(products);
                //SaveChanges();

                //var stocks = new Stock[]
                //{
                //    new Stock{ Name = "Cracow", Balances = new List<ProductQuantity>()
                //    {
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0001-0000-0000-000000000000"), Quantity = 5 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0002-0000-0000-000000000000"), Quantity = 1 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0003-0000-0000-000000000000"), Quantity = 2 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0004-0000-0000-000000000000"), Quantity = 3 }
                //    }
                //    },
                //    new Stock{ Name = "Warsaw", Balances = new List<ProductQuantity>()
                //    {
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0001-0000-0000-000000000000"), Quantity = 10 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0002-0000-0000-000000000000"), Quantity = 10 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0003-0000-0000-000000000000"), Quantity = 20 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0004-0000-0000-000000000000"), Quantity = 30 }
                //    }}
                //};
                //Stocks.AddRange(stocks);
                //SaveChanges();

                var transactions = new Transaction[]
                {
                    new Transaction{ ID = Guid.Parse("00000001-0000-0000-0000-000000000000"), AccountToID = Guid.Parse("00000000-0000-0000-0000-000000000000"), AccountFromID = Guid.Parse("00000000-0000-0000-0001-000000000000"),
                        Amount = 300, Status = TransactionStatus.Confirmed, TimeStamp = DateTime.Now.AddDays(-1) },
                    new Transaction{ ID = Guid.Parse("00000002-0000-0000-0000-000000000000"), AccountToID = Guid.Parse("00000000-0000-0000-0000-000000000000"), AccountFromID = Guid.Parse("00000000-0000-0000-0004-000000000000"),
                        Amount = 350, Status = TransactionStatus.Confirmed, TimeStamp = DateTime.Now.AddDays(-3) },
                    new Transaction{ ID = Guid.Parse("00000003-0000-0000-0000-000000000000"), AccountToID = Guid.Parse("00000000-0000-0000-0001-000000000000"), AccountFromID = Guid.Parse("00000000-0000-0000-0002-000000000000"),
                        Amount = 500, Status = TransactionStatus.Confirmed, TimeStamp = DateTime.Now.AddDays(-1) }
                };
                Transactions.AddRange(transactions);
                SaveChanges();

                //var orders = new Order[]
                //{
                //    new Order{ ID = Guid.Parse("10000000-0000-0000-0000-000000000000"), PurchaseByUserID = Guid.Parse("00000000-0000-0000-0000-000000000001"), PurchaseDate = DateTime.Now.AddDays(-1), Status = OrderStatus.Confirmed,
                //        TransactionID = Guid.Parse("00000001-0000-0000-0000-000000000000"), Basket = new List<ProductQuantity>()
                //    {
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0001-0000-0000-000000000000"), Quantity = 1 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0002-0000-0000-000000000000"), Quantity = 1 }
                //    } },
                //    new Order{ ID = Guid.Parse("20000000-0000-0000-0000-000000000000"), PurchaseByUserID = Guid.Parse("00000000-0000-0000-0000-000000000002"), PurchaseDate = DateTime.Now.AddDays(-3), Status = OrderStatus.Confirmed,
                //        TransactionID = Guid.Parse("00000002-0000-0000-0000-000000000000"), Basket = new List<ProductQuantity>()
                //    {
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0003-0000-0000-000000000000"), Quantity = 2 },
                //        new ProductQuantity{ ProductID = Guid.Parse("00000000-0004-0000-0000-000000000000"), Quantity = 1 }
                //    } }
                //};
                //Orders.AddRange(orders);
                //SaveChanges();
            }

        }

        public DbSet<Account> Accounts { get; set; }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Stock> Stocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        //public DbSet<User> Users { get; set; }
    }
}
