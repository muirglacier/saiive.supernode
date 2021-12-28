using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class CoinPrice
    {
        [JsonProperty("coin")]
        public string Coin { get; set; }

        [JsonProperty("fiat")]
        public double Fiat { get; set; }

        [JsonProperty("idToken")]
        public string IdToken { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
