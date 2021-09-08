using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanOwner
    {
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class OceanOperator
    {
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class OceanCreation
    {
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class OceanResign
    {
        [JsonProperty("tx")]
        public string Tx { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class OceanMasterNodeData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("mintedBlocks")]
        public int MintedBlocks { get; set; }

        [JsonProperty("owner")]
        public OceanOwner Owner { get; set; }

        [JsonProperty("operator")]
        public OceanOperator Operator { get; set; }

        [JsonProperty("creation")]
        public Creation Creation { get; set; }

        [JsonProperty("timelock")]
        public int Timelock { get; set; }

        [JsonProperty("resign")]
        public OceanResign Resign { get; set; }
    }

    public class OceanPage
    {
        [JsonProperty("next")]
        public string Next { get; set; }
    }

    public class OceanMasterNode
    {
        [JsonProperty("data")]
        public List<OceanMasterNodeData> Data { get; set; }

        [JsonProperty("page")]
        public OceanPage Page { get; set; }
    }


}
