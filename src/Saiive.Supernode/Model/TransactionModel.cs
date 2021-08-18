using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saiive.SuperNode.Model
{
    public class TransactionModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("id")] public string IdV1 => Id;

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

        [JsonProperty("spentTxId")]
        public string SpentTxId2 => SpentTxId;

        [JsonProperty("mintTxid")]
        public string MintTxId { get; set; }

        [JsonProperty("mintTxId")]
        public string MintTxId2 => MintTxId;

        [JsonProperty("mintHeight")]
        public int MintHeight { get; set; }

        [JsonProperty("spentHeight")]
        public int SpentHeight { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("value")]
        public ulong Value { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }
        
        [JsonProperty("details")] 
        public TransactionDetailModel Details { get; set; }
    }
}
