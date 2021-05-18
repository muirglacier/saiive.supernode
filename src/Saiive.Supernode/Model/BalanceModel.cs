using Newtonsoft.Json;
using Saiive.SuperNode.Converter;

namespace Saiive.SuperNode.Model
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
