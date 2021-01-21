using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class AccountModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }
    }
}
