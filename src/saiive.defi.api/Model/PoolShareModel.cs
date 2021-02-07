using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class PoolShareModel
    {
        public string Key { get; set; }

        [JsonProperty("poolID")]
        public string PoolID { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("percent")]
        public double Percent { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("totalLiquidity")]
        public double TotalLiquidity { get; set; }
    }
}
