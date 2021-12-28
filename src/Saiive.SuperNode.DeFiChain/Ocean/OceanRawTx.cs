using Newtonsoft.Json;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanRawTx
    {
        [JsonProperty("hex")]
        public string Hex { get; set; }
    }
}
