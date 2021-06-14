using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ProductQuantity
    {
        public Guid ID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
