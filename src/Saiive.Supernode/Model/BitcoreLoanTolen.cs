using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model
{
    public class BitcoreLoanTolen
    {
        [JsonProperty("token")]
        public Dictionary<int, TokenModel> Token { get; set; }
    }
}
