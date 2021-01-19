using Newtonsoft.Json;
using saiive.defi.api.Converter;

namespace saiive.defi.api.Model
{
    public class BalanceModel
    {
        [JsonProperty("confirmed")]
        [JsonConverter(typeof(CoinValueConverter))]
        public double Confirmed { get; set; }

        [JsonProperty("unconfirmed")]
        [JsonConverter(typeof(CoinValueConverter))]
        public double Unconfirmed { get; set; }

        [JsonProperty("balance")]
        [JsonConverter(typeof(CoinValueConverter))]
        public double Balance { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
