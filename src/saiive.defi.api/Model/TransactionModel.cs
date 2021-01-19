using Newtonsoft.Json;
using saiive.defi.api.Converter;

namespace saiive.defi.api.Model
{
    public class TransactionModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("coinbase")]
        public bool Coinbase { get; set; }

        [JsonProperty("mintIndex")]
        public int MintIndex { get; set; }

        [JsonProperty("spentTxid")]
        public string SpentTxId { get; set; }

        [JsonProperty("mintTxid")]
        public string MintTxId { get; set; }

        [JsonProperty("mintHeight")]
        public int MintHeight { get; set; }

        [JsonProperty("spentHeight")]
        public int SpentHeight { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(CoinValueConverter))]
        public double Value { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }
    }
}
