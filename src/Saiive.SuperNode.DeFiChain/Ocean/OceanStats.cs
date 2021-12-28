using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanCount
    {
        [JsonProperty("blocks")]
        public int Blocks { get; set; }

        [JsonProperty("prices")]
        public int Prices { get; set; }

        [JsonProperty("tokens")]
        public int Tokens { get; set; }

        [JsonProperty("masternodes")]
        public int Masternodes { get; set; }
    }

    public class OceanBurned
    {
        [JsonProperty("address")]
        public double Address { get; set; }

        [JsonProperty("emission")]
        public double Emission { get; set; }

        [JsonProperty("fee")]
        public double Fee { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }

    public class OceanTvl
    {
        [JsonProperty("dex")]
        public double Dex { get; set; }

        [JsonProperty("masternodes")]
        public double Masternodes { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }

    public class OceanPrice
    {
        [JsonProperty("usdt")]
        public double Usdt { get; set; }
    }

    public class OceanLocked
    {
        [JsonProperty("weeks")]
        public int Weeks { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("tvl")]
        public double Tvl { get; set; }
    }

    public class OceanMasternodes
    {
        [JsonProperty("locked")]
        public List<OceanLocked> Locked { get; set; }
    }

    public class OceanStatsData
    {
        [JsonProperty("count")]
        public OceanCount Count { get; set; }

        [JsonProperty("burned")]
        public OceanBurned Burned { get; set; }

        [JsonProperty("tvl")]
        public OceanTvl Tvl { get; set; }

        [JsonProperty("price")]
        public OceanPrice Price { get; set; }

        [JsonProperty("masternodes")]
        public OceanMasternodes Masternodes { get; set; }
    }

    public class OceanStats : OceanDataEntity<OceanStatsData>
    {
    }
}
