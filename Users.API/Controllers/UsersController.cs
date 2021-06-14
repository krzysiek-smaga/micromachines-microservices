using System;
using System.Collections.Generic;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.API.Services;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;

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
        public ActionResult<IEnumerable<Order>> GetUserOrders(Guid id)
        {
            return Ok(_userRepository.GetOrders(id));
        }

        [HttpGet]
        [Route("{id}/balance")]
        public ActionResult<IEnumerable<Order>> GetUserBalance(Guid id)
        {
            return Ok(_userRepository.GetUserBalance(id));
        }

        [HttpGet]
        [Route("{id}/products")]
        public ActionResult<IEnumerable<Order>> GetUserProducts(Guid id)
        {
            return Ok(_userRepository.GetProducts(id));
        }

        [HttpGet]
        [Route("{id}/transactions")]
        public ActionResult<decimal> GetUserTransactions(Guid id)
        {
            return Ok(_userRepository.GetTransactionHistory(id));
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
