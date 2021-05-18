using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class FeeEstimateModel
    {
        [JsonProperty("blocks")]
        public int Blocks { get; set; }

        [JsonProperty("feerate")]
        public double FeeRate { get; set; }
    }
}
