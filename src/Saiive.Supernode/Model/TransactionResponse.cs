using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class TransactionRequest
    {
        [JsonProperty("rawTx")]
        public string RawTx { get; set; }
    }

    public class TransactionResponse
    {
        [JsonProperty("txid")]
        public string TxId { get; set; }
    }
}
