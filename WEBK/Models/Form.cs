using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WEBK.Models
{
    public class Form
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty; 

        [JsonProperty("wasteType")]
        public string WasteType { get; set; } = string.Empty; 

        [JsonProperty("quantity")]
        public double Quantity { get; set; } = 0;   
    }
}