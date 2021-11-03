using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class AggregatedAddress
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hid")]
        public string Hid { get; set; }

        [JsonProperty("statistic")]
        public Statistic Statistic { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }
    }

    public class Amount
    {
        [JsonProperty("txIn")]
        public string TxIn { get; set; }

        [JsonProperty("txOut")]
        public string TxOut { get; set; }

        [JsonProperty("unspent")]
        public string Unspent { get; set; }
    }

    public class Statistic
    {
        [JsonProperty("txCount")]
        public int TxCount { get; set; }

        [JsonProperty("txInCount")]
        public int TxInCount { get; set; }

        [JsonProperty("txOutCount")]
        public int TxOutCount { get; set; }
    }
}
