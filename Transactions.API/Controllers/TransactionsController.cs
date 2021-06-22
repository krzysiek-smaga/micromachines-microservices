using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transactions.API.Services;
using Newtonsoft.Json;
using Common.Models;

namespace Transactions.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        //private readonly IUserRepository _userRepository;
        //private readonly IProductRepository _productRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionsController(ILogger<TransactionsController> logger, /*IUserRepository userRepository,
            IProductRepository productRepository,*/ ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _logger = logger;
            //_userRepository = userRepository;
            //_productRepository = productRepository;
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
        [Route("transfer")]
        public ActionResult<Transaction> Create([FromBody] TransactionData transactionData)
        {
            var transaction = new Transaction();
            transaction.TimeStamp = DateTime.Now;
            transaction.Status = TransactionStatus.Unconfirmed;
            transaction.AccountFromID = transactionData.AccountFromID;
            transaction.AccountToID = transactionData.AccountToID;
            transaction.Amount = transactionData.Amount;
            _logger.LogInformation($"Starting transaction!");

            var accountFrom = _accountRepository.GetSingle(x => x.ID == transactionData.AccountFromID);
            var accountTo = _accountRepository.GetSingle(x => x.ID == transactionData.AccountToID);

            if (transactionData.userID != accountFrom.UserID)
            {
                _logger.LogError($"Account From is not yours!");
                return BadRequest("Not your account!");
            }

            if (accountFrom == null || accountTo == null)
            {
                transaction.Status = TransactionStatus.Denied;

                _transactionRepository.Add(transaction);
                _transactionRepository.Save();

                if (accountFrom == null)
                {
                    _logger.LogError($"Account from doesn't exists");
                    return BadRequest($"Error - AccountFrom with id:{transactionData.AccountFromID} - doesn't exists!");
                }

                if (accountTo == null)
                {
                    _logger.LogError($"Account to doesn't exists");
                    return BadRequest($"Error - AccountTo with id:{transactionData.AccountToID} - doesn't exists!");
                }
            }

            if (accountFrom.IsClosed || accountTo.IsClosed || accountFrom.Balance < transactionData.Amount)
            {
                transaction.Status = TransactionStatus.Denied;

                _transactionRepository.Add(transaction);
                _transactionRepository.Save();

                if (accountFrom.IsClosed)
                {
                    _logger.LogError($"Account from is closed");
                    return BadRequest($"Error - AccountFrom with id:{transactionData.AccountFromID} - is closed!");
                }

                if (accountTo.IsClosed)
                {
                    _logger.LogError($"Account to is closed");
                    return BadRequest($"Error - AccountTo with id:{transactionData.AccountToID} - is closed!");
                }

                if (accountFrom.Balance < transactionData.Amount)
                {
                    _logger.LogError($"You don't have enough money on this account");
                    return BadRequest($"Error - AccountFrom with id:{transactionData.AccountFromID} - doesn't enough balance on account!");
                }
            }

            accountFrom.Balance -= transactionData.Amount;
            accountTo.Balance += transactionData.Amount;

            _accountRepository.Edit(accountFrom);
            _accountRepository.Edit(accountTo);
            _accountRepository.Save();

            transaction.Status = TransactionStatus.Confirmed;

            _transactionRepository.Add(transaction);
            _transactionRepository.Save();

            _logger.LogInformation($"Transaction created successfully");
            return CreatedAtAction(nameof(Get), new { id = transaction.ID }, transaction);
        }

        [HttpGet]
        [Route("usertransactions/{userId}")]
        public ActionResult<IEnumerable<Transaction>> GetUserTransactions(Guid userId)
        {
            var userAccounts = _accountRepository.GetAll().Where(x => x.UserID == userId);
            if (userAccounts == null)
            {
                return BadRequest("This user doesn't have account or wrong user ID");
            }

            var userTransactions = _transactionRepository.GetAll().Where(x => userAccounts.Any(y => y.ID == x.AccountToID || y.ID == x.AccountFromID));
            if (userTransactions == null)
            {
                return BadRequest("This user doesn't have transaction yet");
            }

            return Ok(userTransactions);
        }

        [HttpGet]
        [Route("userbalance/{userId}")]
        public ActionResult<decimal> GetUserBalance(Guid userId)
        {
            var userAccounts = _accountRepository.GetAll().Where(x => x.UserID == userId);
            if (userAccounts == null)
            {
                return BadRequest("This user doesn't have account or wrong user ID");
            }

            decimal userBalance = 0;

            foreach (var account in userAccounts)
            {
                userBalance += account.Balance;
            }

            return Ok(userBalance);
        }

        [HttpGet]
        [Route("useraccounts/{userId}")]
        public ActionResult<IEnumerable<Account>> GetUserAccounts(Guid userId)
        {
            var userAccounts = _accountRepository.GetAll().Where(x => x.UserID == userId);
            if (userAccounts == null)
            {
                return BadRequest("This user doesn't have account or wrong user ID");
            }

            return Ok(userAccounts);
        }
    }
}
