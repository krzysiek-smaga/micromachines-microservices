using Codecool.MicroMachines.Services.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transactions.API.Services;

namespace Transactions.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionsController(ILogger<TransactionsController> logger, IUserRepository userRepository,
            IProductRepository productRepository, ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Transaction>> Get()
        {
            return Ok(_transactionRepository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Transaction> Get(Guid id)
        {
            var transaction = _transactionRepository.GetSingle(x => x.ID == id);

            if (transaction == null)
            {
                _logger.LogError($"GET transactions/{id} - Not Found");
                return NotFound();
            }

            _logger.LogInformation($"GET transactions/{id} - Ok");
            return Ok(transaction);
        }

        [HttpPost]
        [Route("amount/{amount}/from/{fromAccountId}/to/{toAccountId}")]
        public ActionResult<Transaction> Create(int amount, Guid fromAccountId, Guid toAccountId)
        {
            var transaction = new Transaction();
            transaction.TimeStamp = DateTime.Now;
            transaction.Status = TransactionStatus.Unconfirmed;
            transaction.AccountFromID = fromAccountId;
            transaction.AccountToID = toAccountId;
            transaction.Amount = amount;

            var accountFrom = _accountRepository.GetSingle(x => x.ID == fromAccountId);
            var accountTo = _accountRepository.GetSingle(x => x.ID == toAccountId);

            if (accountFrom == null || accountTo == null)
            {
                transaction.Status = TransactionStatus.Denied;

                _transactionRepository.Add(transaction);
                _transactionRepository.Save();

                if (accountFrom == null)
                {
                    return BadRequest($"Error - AccountFrom with id:{fromAccountId} - doesn't exists!");
                }

                if (accountTo == null)
                {
                    return BadRequest($"Error - AccountTo with id:{toAccountId} - doesn't exists!");
                }
            }


            if (accountFrom.IsClosed || accountTo.IsClosed || accountFrom.Balance < amount)
            {
                transaction.Status = TransactionStatus.Denied;

                _transactionRepository.Add(transaction);
                _transactionRepository.Save();

                if (accountFrom.IsClosed)
                {
                    return BadRequest($"Error - AccountFrom with id:{fromAccountId} - is closed!");
                }

                if (accountTo.IsClosed)
                {
                    return BadRequest($"Error - AccountTo with id:{toAccountId} - is closed!");
                }

                if (accountFrom.Balance < amount)
                {
                    return BadRequest($"Error - AccountFrom with id:{fromAccountId} - doesn't enough balance on account!");
                }
            }

            accountFrom.Balance -= amount;
            accountTo.Balance += amount;
            transaction.Status = TransactionStatus.Confirmed;

            _transactionRepository.Add(transaction);
            _transactionRepository.Save();

            return CreatedAtAction(nameof(Get), new { id = transaction.ID }, transaction);
        }


    }
}
