using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class EnabledChain
    {
        [JsonProperty("chain")]
        public string Chain { get; set; }

        
        [JsonProperty("network")]
        public string Network { get; set; }
    }
}
