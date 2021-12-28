using System;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class TokenModel
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        
        [JsonProperty("id")] 
        public int Id { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("decimal")]
        public int Decimal { get; set; }

        public int Multiplier => Decimal <= 1 ? 1 : Convert.ToInt32(Math.Pow(10, Decimal));

        [JsonProperty("mintable")]
        public bool Mintable { get; set; }

        [JsonProperty("tradeable")]
        public bool Tradeable { get; set; }

        [JsonProperty("isDAT")]
        public bool IsDAT { get; set; }

        [JsonProperty("isLPS")]
        public bool IsLPS { get; set; }

        [JsonProperty("finalized")]
        public bool Finalized { get; set; }

        [JsonProperty("minted")]
        public double Minted { get; set; }

        [JsonProperty("creationTx")]
        public string CreationTx { get; set; }

        [JsonProperty("creationHeight")]
        public int CreationHeight { get; set; }

        [JsonProperty("destructionTx")]
        public string DestructionTx { get; set; }
        
        [JsonProperty("destructionHeight")]
        public int DestructionHeight { get; set; }
        
        [JsonProperty("collateralAddress")]
        public string CollateralAddress { get; set; }

    }
}
