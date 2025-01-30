using Newtonsoft.Json;

namespace ECommerceBackend.Models
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
