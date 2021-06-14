using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Transaction
    {
        public Guid ID { get; set; }

        public Guid AccountFromID { get; set; }
        public Guid AccountToID { get; set; }

        public DateTime TimeStamp { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
    }

    public enum TransactionStatus { Unconfirmed, Confirmed, Denied }
}
