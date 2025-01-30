using Newtonsoft.Json;

namespace ECommerceBackend.Models
{
	public class Product
	{
		[JsonProperty("id")]
        public string Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
	}
}
