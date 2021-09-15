using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saiive.SuperNode.Model
{
    public class BlockTransactionModel
    {
        [JsonProperty("_id")] 
        public string Id { get; set; }

        [JsonProperty("txid")] 
        public string Txid { get; set; }

        [JsonProperty("network")] 
        public string Network { get; set; }

        [JsonProperty("chain")] 
        public string Chain { get; set; }

        [JsonProperty("blockHeight")] 
        public long BlockHeight { get; set; }

        [JsonProperty("blockHash")] 
        public string BlockHash { get; set; }

        [JsonProperty("blockTime")] 
        public DateTime BlockTime { get; set; }

        [JsonProperty("blockTimeNormalized")] 
        public DateTime BlockTimeNormalized { get; set; }

        [JsonProperty("coinbase")] 
        public bool Coinbase { get; set; }

        [JsonProperty("locktime")]
        public long Locktime { get; set; }

        [JsonProperty("inputCount")] 
        public long InputCount { get; set; }

        [JsonProperty("outputCount")] 
        public long OutputCount { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("fee")] 
        public long Fee { get; set; }

        [JsonProperty("value")]
        public ulong Value { get; set; }

        [JsonProperty("isCustom")]
        public bool IsCustom { get; set; }

        [JsonProperty("isCustomTxApplied")]
        public bool IsCustomTxApplied { get; set; }

        [JsonProperty("txType")]
        public object TxType { get; set; }

        [JsonProperty("customData")] 
        public JObject CustomData { get; set; }

        [JsonProperty("confirmations")] 
        public long Confirmations { get; set; }

        [JsonProperty("details")]
        public TransactionDetailModel Details { get; set; }

    }
}
