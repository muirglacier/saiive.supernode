using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class BitcoreCollateral
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("factor")]
        public int Factor { get; set; }

        [JsonProperty("fixedIntervalPriceId")]
        public string FixedIntervalPriceId { get; set; }

        [JsonProperty("activateAfterBlock")]
        public int ActivateAfterBlock { get; set; }
    }
}
