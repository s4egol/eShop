using Newtonsoft.Json;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderApi
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public OrderAddress Address { get; set; }
        public OrderItem[] Items { get; set; }
        public string FinalPrice { get; set; }
    }
}
