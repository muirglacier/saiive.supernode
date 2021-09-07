using Newtonsoft.Json;

namespace Saiive.SuperNode.DeFiChain.Ocean
{
    public class OceanTokens
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("displaySymbol")]
        public string DisplaySymbol { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isDAT")]
        public bool IsDat { get; set; }

        [JsonProperty("isLPS")]
        public bool IsLps { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }

        [JsonProperty("destruction")]
        public Creation Destruction { get; set; }

        [JsonProperty("finalized")]
        public bool Finalized { get; set; }

        [JsonProperty("minted")]
        public string Minted { get; set; }

        [JsonProperty("creation")]
        public Creation Creation { get; set; }

        [JsonProperty("decimal")]
        public int Decimal { get; set; }

        [JsonProperty("mintable")]
        public bool Mintable { get; set; }

        [JsonProperty("tradeable")]
        public bool Tradeable { get; set; }

    }
}
