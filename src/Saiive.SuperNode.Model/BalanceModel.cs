using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class BalanceModel
    {
        [JsonProperty("confirmed")]
        public ulong Confirmed { get; set; }

        [JsonProperty("unconfirmed")]
        public ulong Unconfirmed { get; set; }

        [JsonProperty("balance")]
        public ulong Balance { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
