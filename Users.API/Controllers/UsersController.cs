using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.API.Services;
using Newtonsoft.Json;

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
            string url = "http://localhost:50629/orders/userorders/" + id;

            _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<IEnumerable<Order>>(content));
            }

            return BadRequest();
        }

        //[HttpGet]
        //[Route("{id}/balance")]
        //public ActionResult<IEnumerable<Order>> GetUserBalance(Guid id)
        //{
        //    return Ok(_userRepository.GetUserBalance(id));
        //}

        //[HttpGet]
        //[Route("{id}/products")]
        //public ActionResult<IEnumerable<Order>> GetUserProducts(Guid id)
        //{
        //    return Ok(_userRepository.GetProducts(id));
        //}

        [HttpGet]
        [Route("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(Guid id)
        {
            string url = "http://localhost:64634/transactions/usertransactions/" + id;

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
        [Route("{id}/addtobasket/{productId}/{quantity}")]
        public ActionResult<User> AddToBasket(Guid id, Guid productId, int quantity)
        {
            var productToAdd = new ProductQuantity { ProductID = productId, Quantity = quantity };

            var user = _userRepository.GetSingle(x => x.ID == id);
            user.Basket.Add(productToAdd);

            _userRepository.Edit(user);
            _userRepository.Save();

            return Ok(user);
        }
    }
}
