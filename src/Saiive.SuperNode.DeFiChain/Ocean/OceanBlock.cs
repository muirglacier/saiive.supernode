using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanBlock : OceanDataEntity<OceanBlockData>
    {

    }
    public class OceanBlockList : OceanDataEntity<List<OceanBlockData>>
    {

    }

    public class OceanBlockData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("previousHash")]
        public string PreviousHash { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("medianTime")]
        public int MedianTime { get; set; }

        [JsonProperty("transactionCount")]
        public int TransactionCount { get; set; }

        [JsonProperty("difficulty")]
        public double Difficulty { get; set; }

        [JsonProperty("masternode")]
        public string Masternode { get; set; }

        [JsonProperty("minter")]
        public string Minter { get; set; }

        [JsonProperty("minterBlockCount")]
        public int MinterBlockCount { get; set; }

        [JsonProperty("stakeModifier")]
        public string StakeModifier { get; set; }

        [JsonProperty("merkleroot")]
        public string Merkleroot { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("sizeStripped")]
        public int SizeStripped { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }
    }
}
