using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Account
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsClosed { get; set; }

        public Guid UserID { get; set; }
    }
}
