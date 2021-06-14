using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ITransactionRepository _transactionRepository;

        public OrdersController(ILogger<OrdersController> logger, IUserRepository userRepository, IProductRepository productRepository,
            IOrderRepository orderRepository, IAccountRepository accountRepository, IStockRepository stockRepository,
            ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _stockRepository = stockRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return Ok(_orderRepository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Order> Get(Guid id)
        {
            var order = _orderRepository.GetSingle(x => x.ID == id);

            if (order == null)
            {
                _logger.LogError($"GET orders/{id} - Not Found");
                return NotFound();
            }

            _logger.LogInformation($"GET orders/{id} - Ok");
            return Ok(order);
        }

        [HttpPost]
        [Route("makeorder/{userId}")]
        public ActionResult<Order> Create(Guid userId)
        {
            var user = _userRepository.GetSingle(x => x.ID == userId);

            var order = new Order();
            order.PurchaseByUserID = userId;
            order.Status = OrderStatus.Pending;
            order.PurchaseDate = DateTime.Now;
            order.Basket = new List<ProductQuantity>();

            foreach (var product in user.Basket)
            {
                var newProductInBasket = new ProductQuantity();
                newProductInBasket.ProductID = product.ProductID;
                newProductInBasket.Quantity = product.Quantity;

                order.Basket.Add(newProductInBasket);
            }

            user.Basket = new List<ProductQuantity>();

            var transaction = new Transaction();
            transaction.TimeStamp = DateTime.Now;
            transaction.Status = TransactionStatus.Unconfirmed;

            var storeAccount = _accountRepository.GetSingle(x => x.Name == "StoreAccount");
            transaction.AccountToID = storeAccount.ID;

            decimal amount = 0;
            foreach (var product in order.Basket)
            {
                amount += product.Quantity * _productRepository.GetSingle(x => x.ID == product.ProductID).Price;
            }

            transaction.Amount = amount;

            var userAccount = _accountRepository.GetSingle(x => x.UserID == userId &&
                                        x.Balance >= transaction.Amount &&
                                        x.IsClosed == false);

            if (userAccount == null)
            {
                userAccount = _accountRepository.GetAll().FirstOrDefault(x => x.ID == userId);
                transaction.AccountFromID = userAccount.ID;

                transaction.Status = TransactionStatus.Denied;

                _transactionRepository.Add(transaction);
                _transactionRepository.Save();
                order.TransactionID = transaction.ID;

                order.Status = OrderStatus.Cancelled;

                _orderRepository.Add(order);
                _orderRepository.Save();
                return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
            }

            var stocks = _stockRepository.GetAll().ToList();

            foreach (var product in order.Basket)
            {
                var stock = stocks.FirstOrDefault(x => x.Balances.Any(y => y.ProductID == product.ProductID && y.Quantity >= product.Quantity));
                //var stock = _stockRepository.GetSingle(x => x.Balances.Any(y => y.ProductID == product.ProductID && y.Quantity >= product.Quantity));

                if (stock == null)
                {
                    userAccount = _accountRepository.GetAll().FirstOrDefault(x => x.ID == userId);
                    transaction.AccountFromID = userAccount.ID;

                    transaction.Status = TransactionStatus.Denied;

                    _transactionRepository.Add(transaction);
                    _transactionRepository.Save();
                    order.TransactionID = transaction.ID;

                    order.Status = OrderStatus.Cancelled;

                    _logger.LogInformation($"POST orders/makeorder/{userId} - There is not enough products on stocks! Order Cancelled!");

                    _orderRepository.Add(order);
                    _orderRepository.Save();
                    return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
                }

                stock.Balances.First(x => x.ProductID == product.ProductID).Quantity -= product.Quantity;
                _stockRepository.Edit(stock);
                _stockRepository.Save();
            }

            transaction.AccountFromID = userAccount.ID;
            transaction.Status = TransactionStatus.Confirmed;

            _transactionRepository.Add(transaction);
            _transactionRepository.Save();
            order.TransactionID = transaction.ID;

            userAccount.Balance -= transaction.Amount;
            _accountRepository.Edit(userAccount);
            storeAccount.Balance += transaction.Amount;
            _accountRepository.Edit(storeAccount);

            order.Status = OrderStatus.Confirmed;

            _orderRepository.Add(order);
            _orderRepository.Save();
            return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
        }
    }
}
