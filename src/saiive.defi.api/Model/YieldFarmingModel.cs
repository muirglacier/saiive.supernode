using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class YieldFramingModel
    {
        [JsonProperty("apr")]
        public double Apr { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("poolRewards")]
        public List<string> PoolRewards { get; set; }

        [JsonProperty("pairLink")]
        public string PairLink { get; set; }

        [JsonProperty("apy")]
        public double Apy { get; set; }

        [JsonProperty("idTokenA")]
        public string IdTokenA { get; set; }

        [JsonProperty("idTokenB")]
        public string IdTokenB { get; set; }

        [JsonProperty("totalStaked")]
        public double TotalStaked { get; set; }

        [JsonProperty("poolPairId")]
        public string PoolPairId { get; set; }

        [JsonProperty("reserveA")]
        public double ReserveA { get; set; }

        [JsonProperty("reserveB")]
        public double ReserveB { get; set; }

        [JsonProperty("volumeA")]
        public double VolumeA { get; set; }

        [JsonProperty("volumeB")]
        public double VolumeB { get; set; }

        [JsonProperty("tokenASymbol")]
        public string TokenASymbol { get; set; }

        [JsonProperty("tokenBSymbol")]
        public string TokenBSymbol { get; set; }

        [JsonProperty("priceA")]
        public double PriceA { get; set; }

        [JsonProperty("priceB")]
        public double PriceB { get; set; }
    }
}
