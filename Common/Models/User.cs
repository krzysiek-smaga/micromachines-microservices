using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class User
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }

        //public IList<Guid> Accounts { get; set; }
        public IList<ProductQuantity> Basket { get; set; } = new List<ProductQuantity>();
    }
}
