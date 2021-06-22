using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class TransactionData
    {
        public Guid userID { get; set; }
        public Guid AccountFromID { get; set; }
        public Guid AccountToID { get; set; }
        public decimal Amount { get; set; }
    }
}
