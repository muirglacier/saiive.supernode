using Newtonsoft.Json;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    internal class OceanBalance
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
