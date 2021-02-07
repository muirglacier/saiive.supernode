using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
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
