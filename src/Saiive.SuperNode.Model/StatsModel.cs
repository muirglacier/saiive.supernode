using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model
{
    public class BurnInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("tokens")]
        public List<string> Tokens { get; set; }

        [JsonProperty("feeburn")]
        public int Feeburn { get; set; }

        [JsonProperty("emissionburn")]
        public string Emissionburn { get; set; }
    }

    public class Supply
    {
        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("circulation")]
        public double Circulation { get; set; }

        [JsonProperty("foundation")]
        public int Foundation { get; set; }

        [JsonProperty("community")]
        public double Community { get; set; }
    }

    public class InitDist
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("totalPercent")]
        public int TotalPercent { get; set; }

        [JsonProperty("foundation")]
        public int Foundation { get; set; }

        [JsonProperty("foundationPercent")]
        public int FoundationPercent { get; set; }

        [JsonProperty("circulation")]
        public int Circulation { get; set; }

        [JsonProperty("circulationPercent")]
        public int CirculationPercent { get; set; }
    }

    public class Tokens
    {
        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("supply")]
        public Supply Supply { get; set; }

        [JsonProperty("initDist")]
        public InitDist InitDist { get; set; }
    }

    public class Rewards
    {
        [JsonProperty("anchorPercent")]
        public double AnchorPercent { get; set; }

        [JsonProperty("liquidityPoolPercent")]
        public double LiquidityPoolPercent { get; set; }

        [JsonProperty("communityPercent")]
        public double CommunityPercent { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("community")]
        public double Community { get; set; }

        [JsonProperty("minter")]
        public double Minter { get; set; }

        [JsonProperty("anchorReward")]
        public double AnchorReward { get; set; }

        [JsonProperty("liquidityPool")]
        public int LiquidityPool { get; set; }

        [JsonProperty("masternode")]
        public double Masternode { get; set; }

        [JsonProperty("anchor")]
        public double Anchor { get; set; }

        [JsonProperty("liquidity")]
        public double Liquidity { get; set; }

        [JsonProperty("swap")]
        public double Swap { get; set; }

        [JsonProperty("futures")]
        public double Futures { get; set; }

        [JsonProperty("options")]
        public double Options { get; set; }

        [JsonProperty("unallocated")]
        public double Unallocated { get; set; }
    }

    public class ListCommunities
    {
        [JsonProperty("AnchorReward")]
        public double AnchorReward { get; set; }

        [JsonProperty("Burnt")]
        public string Burnt { get; set; }
    }

    public class StatsModel
    {
        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("blockHeight")]
        public int BlockHeight { get; set; }

        [JsonProperty("bestBlockHash")]
        public string BestBlockHash { get; set; }

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("medianTime")]
        public int MedianTime { get; set; }

        [JsonProperty("burnInfo")]
        public BurnInfo BurnInfo { get; set; }

        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("tokens")]
        public Tokens Tokens { get; set; }

        [JsonProperty("rewards")]
        public Rewards Rewards { get; set; }

        [JsonProperty("listCommunities")]
        public ListCommunities ListCommunities { get; set; }
    }


}
