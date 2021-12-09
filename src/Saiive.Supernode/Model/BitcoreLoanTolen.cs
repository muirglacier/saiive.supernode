using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model
{
    public class BitcoreLoanToken
    {
        [JsonProperty("token")]
        public Dictionary<int, TokenModel> Token { get; set; }

        [JsonProperty("fixedIntervalPriceId")]
        public string FixedIntervalPriceId { get; set; }

        [JsonProperty("intereste")]
        public int Interest { get; set; }
    }

}
