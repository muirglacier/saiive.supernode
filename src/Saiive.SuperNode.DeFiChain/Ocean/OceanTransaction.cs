using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    internal class OceanTransactions : OceanDataEntity<List<OceanTransactionData>>
    {
    }

    public class Block
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("medianTime")]
        public int MedianTime { get; set; }
    }

    public class Script
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("hex")]
        public string Hex { get; set; }
    }

    public class Vout
    {
        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("tokenId")]
        public int TokenId { get; set; }
    }
    public class Vin
    {
        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

    }

    public class OceanTransactionData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hid")]
        public string Hid { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("script")]
        public Script Script { get; set; }

        [JsonProperty("vout")]
        public Vout Vout { get; set; }

        [JsonProperty("vin")]
        public Vin Vin { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
