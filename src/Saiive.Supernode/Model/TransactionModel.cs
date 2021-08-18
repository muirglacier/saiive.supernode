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
        public ulong Value { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }
        
        [JsonProperty("details")] 
        public TransactionDetailModel Details { get; set; }

        [JsonProperty("isCustom")] 
        public bool? IsCustom { get; set; }

        [JsonProperty("isCustomTxApplied")] 
        public bool? IsCustomTxApplied { get; set; }

        [JsonProperty("txType")] 
        public string? TxType { get; set; }

        [JsonProperty("customData")] 
        public JObject? CustomData { get; set; }

        [JsonProperty("fee")]
        public int? Fee { get; set; }

        [JsonProperty("size")]
        public int? Size { get; set; }

        [JsonProperty("locktime")] 
        public int? LockTime { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockHeight")]
        public Int64? BlockHeight { get; set; }

        [JsonProperty("bkockTime")] 
        public string BlockTime { get; set; }

        [JsonProperty("blockTimeNormalized")]
        public string BlockTimeNormalized { get; set; }

        [JsonProperty("inputCount")] 
        public int? InputCount { get; set; }

        [JsonProperty("outputCount")]
        public int? OutputCount { get; set; }

    }
}
