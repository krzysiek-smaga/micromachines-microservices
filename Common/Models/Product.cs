using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Product
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Guid CategoryID { get; set; }
    }
}
