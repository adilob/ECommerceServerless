using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ECommerceBackend.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        public List<Product> Products { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
    }
}
