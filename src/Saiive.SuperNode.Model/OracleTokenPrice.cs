using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class OracleTokenPrice
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("ok")]
        public bool Ok { get; set; }
    }
}
