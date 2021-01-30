using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class PoolPairModel
    {
        public string ID { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("idTokenA")]
        public string IdTokenA { get; set; }

        [JsonProperty("idTokenB")]
        public string IdTokenB { get; set; }

        [JsonProperty("reserveA")]
        public double ReserveA { get; set; }

        [JsonProperty("reserveB")]
        public double ReserveB { get; set; }

        [JsonProperty("comission")]
        public double Commision { get; set; }

        [JsonProperty("totalLiquidity")]
        public double TotalLiquidity { get; set; }

        [JsonProperty("reserveA/reserveB")]
        public double ReserveADivReserveB { get; set; }

        [JsonProperty("reserveB/reserveA")]
        public double ReserveBDivReserveA { get; set; }

        [JsonProperty("tradeEnabled")]
        public bool TradeEnabled { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("blockCommissionA")]
        public double BlockCommissionA { get; set; }

        [JsonProperty("blockCommissionB")]
        public double BlockCommissionB { get; set; }

        [JsonProperty("rewardPct")]
        public double RewardPct { get; set; }

        [JsonProperty("customRewards")]
        public IList<string> CustomRewards { get; set; }

        [JsonProperty("creationTx")]
        public string CreationTx { get; set; }

        [JsonProperty("creationHeight")]
        public int CreationHeight { get; set; }
    }
}
