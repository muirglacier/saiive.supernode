using Newtonsoft.Json;
using System.Collections.Generic;

namespace saiive.defi.api.Model
{
    public class AccountHistory
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("blockHeight")]
        public int BlockHeight { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockTime")]
        public int BlockTime { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("poolID")]
        public string PoolID { get; set; }

        [JsonProperty("amounts")]
        public IList<string> Amounts { get; set; }
    }
}
