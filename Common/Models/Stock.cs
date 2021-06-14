using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Stock
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public IList<ProductQuantity> Balances { get; set; }
    }
}
