using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Order
    {
        public Guid ID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public IList<ProductQuantity> Basket { get; set; }

        public Guid PurchaseByUserID { get; set; }
        public Guid TransactionID { get; set; }

        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus { Pending, Denied, Confirmed, Cancelled }
}
