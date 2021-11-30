using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryOrderProcessor.Models
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; }
        public OrderAddress Address { get; set; }
        public OrderItem[] Items { get; set; }
        public string FinalPrice { get; set; }
    }
}
