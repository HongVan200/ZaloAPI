
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZaloMiniAppAPI
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }



}
