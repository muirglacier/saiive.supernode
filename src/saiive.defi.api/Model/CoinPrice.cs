using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class CoinPrice
    {
        [JsonProperty("coin")]
        public string Coin { get; set; }

        [JsonProperty("fiat")]
        public double fiat { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
