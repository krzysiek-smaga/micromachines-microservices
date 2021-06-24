using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orders.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        //private readonly IUserRepository _userRepository;
        //private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        //private readonly IAccountRepository _accountRepository;
        //private readonly IStockRepository _stockRepository;
        //private readonly ITransactionRepository _transactionRepository;

        private HttpClient _httpClient;

        public OrdersController(ILogger<OrdersController> logger, /*IUserRepository userRepository, IProductRepository productRepository,*/
            IOrderRepository orderRepository/*, IAccountRepository accountRepository, IStockRepository stockRepository,
            //ITransactionRepository transactionRepository*/)
        {
            _logger = logger;
            //_userRepository = userRepository;
            //_productRepository = productRepository;
            _orderRepository = orderRepository;
            //_accountRepository = accountRepository;
            //_stockRepository = stockRepository;
            //_transactionRepository = transactionRepository;

            _httpClient = new HttpClient();
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

        [HttpGet]
        [Route("userorders/{userId}")]
        public ActionResult<IEnumerable<Order>> GetUserOrders(Guid userId)
        {
            return Ok(_orderRepository.GetAll().Where(x => x.PurchaseByUserID == userId).ToList());
        }

        [HttpPost]
        [Route("make_order/{userId}")]
        public async  Task<ActionResult<Order>> Create([FromRoute] Guid userId, [FromBody] List<ProductQuantity> basket)
        {
            //var user = _userRepository.GetSingle(x => x.ID == userId);

            var order = new Order();
            order.PurchaseByUserID = userId;
            order.Status = OrderStatus.Pending;
            order.PurchaseDate = DateTime.Now;
            order.Basket = new List<ProductQuantity>();

            foreach (var product in basket)
            {
                var newProductInBasket = new ProductQuantity();
                newProductInBasket.ProductID = product.ProductID;
                newProductInBasket.Quantity = product.Quantity;

                order.Basket.Add(newProductInBasket);
            }

            //user.Basket = new List<ProductQuantity>();

            //var transaction = new Transaction();
            //transaction.TimeStamp = DateTime.Now;
            //transaction.Status = TransactionStatus.Unconfirmed;

            //var storeAccount = _accountRepository.GetSingle(x => x.Name == "StoreAccount");
            //transaction.AccountToID = storeAccount.ID;

            var storeAccountId = Guid.Parse("00000000-0000-0000-1111-000000000000");

            decimal amount = await CalculateAmount(basket);


            // next TODO - zapytanie do transaction api o pierwsze lepsze konto użytkownika na którym będzie wystarczająco środków

            var userAccount = await GetUserAccountForTransaction(userId, amount);
            var userAccountId = userAccount.ID;
            

            //transaction.Amount = amount;

            //var userAccount = _accountRepository.GetSingle(x => x.UserID == userId &&
            //                            x.Balance >= transaction.Amount &&
            //                            x.IsClosed == false);

            //if (userAccount == null)
            //{
            //    userAccount = _accountRepository.GetAll().FirstOrDefault(x => x.ID == userId);
            //    transaction.AccountFromID = userAccount.ID;

            //    transaction.Status = TransactionStatus.Denied;

            //    _transactionRepository.Add(transaction);
            //    _transactionRepository.Save();
            //    order.TransactionID = transaction.ID;

            //    order.Status = OrderStatus.Cancelled;

            //    _orderRepository.Add(order);
            //    _orderRepository.Save();
            //    return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
            //}

            var transactionData = new TransactionData() { userID = userId, AccountFromID = userAccountId, AccountToID = storeAccountId, Amount = amount };

            //var stocks = _stockRepository.GetAll().ToList();

            var stocks = await GetStocks();
            
            if (!AreProductsAvailable(stocks, basket))
            {
                BadRequest("Not enough products on stock");
            }

            foreach (var product in order.Basket)
            {
                var stock = stocks.FirstOrDefault(x => x.Balances.Any(y => y.ProductID == product.ProductID && y.Quantity >= product.Quantity));
                //var stock = _stockRepository.GetSingle(x => x.Balances.Any(y => y.ProductID == product.ProductID && y.Quantity >= product.Quantity));

                //if (stock == null)
                //{
                //    userAccount = _accountRepository.GetAll().FirstOrDefault(x => x.ID == userId);
                //    transaction.AccountFromID = userAccount.ID;

                //    transaction.Status = TransactionStatus.Denied;

                //    _transactionRepository.Add(transaction);
                //    _transactionRepository.Save();
                //    order.TransactionID = transaction.ID;

                //    order.Status = OrderStatus.Cancelled;

                //    _logger.LogInformation($"POST orders/makeorder/{userId} - There is not enough products on stocks! Order Cancelled!");

                //    _orderRepository.Add(order);
                //    _orderRepository.Save();
                //    return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
                //}

                stock.Balances.First(x => x.ProductID == product.ProductID).Quantity -= product.Quantity;
                //_stockRepository.Edit(stock);
                //_stockRepository.Save();

                await UpdateStock(stock);
            }

            var transaction = await AddTransaction(transactionData);

            //transaction.AccountFromID = userAccount.ID;
            //transaction.Status = TransactionStatus.Confirmed;

            //_transactionRepository.Add(transaction);
            //_transactionRepository.Save();
            //order.TransactionID = transaction.ID;

            order.TransactionID = transaction.ID;

            //userAccount.Balance -= transaction.Amount;
            //_accountRepository.Edit(userAccount);
            //storeAccount.Balance += transaction.Amount;
            //_accountRepository.Edit(storeAccount);

            order.Status = OrderStatus.Confirmed;

            _orderRepository.Add(order);
            _orderRepository.Save();
            return CreatedAtAction(nameof(Get), new { id = order.ID }, order);
        }

        private async Task<Transaction> AddTransaction(TransactionData transactionData)
        {
            string url = "http://transactions.api/transactions/transfer";

            var json = JsonConvert.SerializeObject(transactionData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Transaction>(content);
            }

            return null;
        }

        private async Task<ActionResult> UpdateStock(Stock stock)
        {
            string url = "http://products.api/products/update_stock";

            var json = JsonConvert.SerializeObject(stock);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Something bad with stock update!");
            }

            return Ok();
        }

        private bool AreProductsAvailable(IEnumerable<Stock> stocks, List<ProductQuantity> basket)
        {
            foreach (var product in basket)
            {
                var stock = stocks.FirstOrDefault(x => x.Balances.Any(y => y.ProductID == product.ProductID && y.Quantity >= product.Quantity));

                if (stock == null)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<IEnumerable<Stock>> GetStocks()
        {
            string stocksUrl = "http://products.api/products/stocks/";
            var responseStocks = await _httpClient.GetAsync(stocksUrl);
            if (!responseStocks.IsSuccessStatusCode)
            {
                return null;
            }

            var contentStocks = await responseStocks.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Stock>>(contentStocks);
        }

        private async Task<Account> GetUserAccountForTransaction(Guid userId, decimal amount)
        {
            string userAccountUrl = "http://transactions.api/transactions/useraccounts/" + userId;

            var responseUserAccount = await _httpClient.GetAsync(userAccountUrl);
            if (!responseUserAccount.IsSuccessStatusCode)
            {
                return null;
            }
            var contentUserAccount = await responseUserAccount.Content.ReadAsStringAsync();
            var userAccounts = JsonConvert.DeserializeObject<IEnumerable<Account>>(contentUserAccount);
            var userAccountWithEnoughMoney = userAccounts.FirstOrDefault(x => x.UserID == userId &&
                                        x.Balance >= amount &&
                                        x.IsClosed == false);
            if (userAccountWithEnoughMoney == null)
            {
                return null;
            }

            return userAccountWithEnoughMoney;
        }

        private async Task<decimal> CalculateAmount(List<ProductQuantity> basket)
        {
            decimal amount = 0;
            foreach (var productInBasket in basket)
            {
                string urlProducts = "http://products.api/products/" + productInBasket.ProductID;

                var responseProducts = await _httpClient.GetAsync(urlProducts);

                var contentProducts = await responseProducts.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(contentProducts);

                amount += productInBasket.Quantity * product.Price;
            }

            return amount;
        }
    }
}
