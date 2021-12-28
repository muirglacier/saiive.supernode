using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanToken
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("displaySymbol")]
        public string DisplaySymbol { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("reserve")]
        public string Reserve { get; set; }

        [JsonProperty("blockCommission")]
        public string BlockCommission { get; set; }
    }
    public class PriceRatio
    {
        [JsonProperty("ab")]
        public string Ab { get; set; }

        [JsonProperty("ba")]
        public string Ba { get; set; }
    }
    public class TotalLiquidity
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("usd")]
        public string Usd { get; set; }
    }

    public class Creation
    {
        [JsonProperty("tx")]
        public string Tx { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Apr
    {
        [JsonProperty("reward")]
        public double? Reward { get; set; }

        [JsonProperty("total")]
        public double? Total { get; set; }
    }

    public class OceanPoolPairData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("tokenA")]
        public OceanToken TokenA { get; set; }

        [JsonProperty("tokenB")]
        public OceanToken TokenB { get; set; }

        [JsonProperty("priceRatio")]
        public PriceRatio PriceRatio { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("totalLiquidity")]
        public TotalLiquidity TotalLiquidity { get; set; }

        [JsonProperty("tradeEnabled")]
        public bool TradeEnabled { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("rewardPct")]
        public string RewardPct { get; set; }

        [JsonProperty("customRewards")]
        public List<string> CustomRewards { get; set; }

        [JsonProperty("creation")]
        public Creation Creation { get; set; }

        [JsonProperty("apr")]
        public Apr Apr { get; set; }
    }

    public class OceanPoolPair : OceanDataEntity<List<OceanPoolPairData>>
    {
    }

    public class OceanDataEntity<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("page")]
        public OceanPage Page { get; set; }
    }
}
