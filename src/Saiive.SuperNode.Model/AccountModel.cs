using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
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

        [JsonProperty("isDAT")]
        public bool IsDAT { get; set; }
        
        [JsonProperty("isLPS")]
        public bool IsLPS { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }
    }
}
