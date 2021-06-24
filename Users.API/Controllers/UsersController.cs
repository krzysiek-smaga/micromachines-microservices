using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.API.Services;
using Newtonsoft.Json;
using System.Text;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private HttpClient _httpClient;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Ok(_userRepository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<User> Get(Guid id)
        {
            return Ok(_userRepository.GetSingle(u => u.ID == id));
        }

        [HttpPost]
        public ActionResult<User> Create([FromBody] User user)
        {
            _userRepository.Add(user);
            _userRepository.Save();

            return Ok(user);
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult<User> Update([FromBody] User user)
        {
            _userRepository.Edit(user);
            _userRepository.Save();

            return Ok(user);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Remove(Guid id)
        {
            _userRepository.Delete(_userRepository.GetSingle(u => u.ID == id));

            return NoContent();
        }

        [HttpGet]
        [Route("{id}/orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(Guid id)
        {
            string url = "http://orders.api/orders/userorders/" + id;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Order>>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{id}/balance")]
        public async Task<ActionResult<decimal>> GetUserBalance(Guid id)
        {
            string url = "http://transactions.api/transactions/userbalance/" + id;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<decimal>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{id}/accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetUserAccounts(Guid id)
        {
            string url = "http://transactions.api/transactions/useraccounts/" + id;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Account>>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("get_stocks")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocks()
        {
            string url = "http://products.api/products/stocks/";

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Stock>>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("get_categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            string url = "http://products.api/products/categories/";

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Category>>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("get_products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string categoryName)
        {
            string url = "http://products.api/products";

            if (!string.IsNullOrEmpty(categoryName))
            {
                url += "?categoryName=" + categoryName.ToLower();
            }

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Product>>(content));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{id}/user_products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetUserProducts(Guid id)
        {
            var user = _userRepository.GetSingle(x => x.ID == id);

            if (user == null)
            {
                return BadRequest();
            }

            var userProducts = new List<Product>();

            foreach (var productInBasket in user.Basket)
            {
                string url = "http://products.api/products/" + productInBasket.ProductID;

                _httpClient = new HttpClient();

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<Product>(content);

                    userProducts.Add(product);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(userProducts);
        }

        [HttpGet]
        [Route("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(Guid id)
        {
            string url = "http://transactions.api/transactions/usertransactions/" + id;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Transaction>>(content));
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("{id}/add_to_basket/{productId}/{quantity}")]
        public async Task<ActionResult<User>> AddToBasket(Guid id, Guid productId, int quantity)
        {
            string url = "http://products.api/products/" + productId;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var productToAdd = new ProductQuantity { ProductID = productId, Quantity = quantity };

                var user = _userRepository.GetSingle(x => x.ID == id);
                user.Basket.Add(productToAdd);

                _userRepository.Edit(user);
                _userRepository.Save();

                return Ok(user);
            }

            return BadRequest("Product not found");
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<ActionResult<Transaction>> Transfer([FromBody] TransactionData transactionData)
        {
            string url = "http://transactions.api/transactions/transfer";

            _httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(transactionData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<Transaction>(content));
            }

            return BadRequest("Error :)");
        }

        [HttpPost]
        [Route("{userId}/make_order")]
        public async Task<ActionResult<Order>> MakeOrder([FromRoute] Guid userId)
        {
            var user = _userRepository.GetSingle(x => x.ID == userId);

            if (user == null)
            {
                return BadRequest("User doesn't exists");
            }

            if (user.Basket.Count == 0 || user.Basket == null)
            {
                return BadRequest("Basket empty, nothing to order. Add products to basket first");
            }

            string url = "http://orders.api/orders/make_order/" + userId;

            _httpClient = new HttpClient();

            var basket = new List<ProductQuantity>();

            foreach (var product in user.Basket)
            {
                var newProductInBasket = new ProductQuantity();
                newProductInBasket.ProductID = product.ProductID;
                newProductInBasket.Quantity = product.Quantity;

                basket.Add(newProductInBasket);
            }

            user.Basket = new List<ProductQuantity>();

            var json = JsonConvert.SerializeObject(basket);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<Order>(content));
            }

            return BadRequest("Damn! :)");
        }
    }
}
