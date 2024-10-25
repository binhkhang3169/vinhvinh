using Newtonsoft.Json;

namespace WEBK.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("price")]
        public double Price { get; set; } = 0;

        [JsonProperty("quantitySold")]
        public int QuantitySold { get; set; } = 0;
    }
}
