using System;
using System.Collections.Generic;
using System.Text;

namespace OrderItemsReserver.Models
{
    public class OrderItem
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public int Count { get; set; }
    }
}
