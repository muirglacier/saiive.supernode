using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanTransactionDetail : OceanDataEntity<OceanTransactionDetailData>
    {

    }

    public class OceanVOutDetail : OceanDataEntity<IList<OceanVOutData>>
    {

    }
    public class OceanVInDetail : OceanDataEntity<IList<OceanVOutData>>
    {

    }



    public class OceanVin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("tokenId")]
        public int TokenId { get; set; }

        [JsonProperty("script")]
        public Script Script { get; set; }
    }

    public class OceanVout
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("tokenId")]
        public int TokenId { get; set; }
    }

    public class OceanVOutData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("vout")]
        public Vout Vout { get; set; }

        [JsonProperty("script")]
        public Script Script { get; set; }

        [JsonProperty("txInWitness")]
        public List<string> TxInWitness { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }
    }


    public class OceanBlockDetail
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

    public class OceanTransactionDetailData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("vSize")]
        public int VSize { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("lockTime")]
        public int LockTime { get; set; }

        [JsonProperty("vinCount")]
        public int VinCount { get; set; }

        [JsonProperty("voutCount")]
        public int VoutCount { get; set; }
    }
}
