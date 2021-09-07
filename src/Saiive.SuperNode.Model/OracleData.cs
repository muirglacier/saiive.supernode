using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class PriceFeed
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }


    public class OracleData
    {
        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("oracleid")]
        public string Oracleid { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("priceFeeds")]
        public List<PriceFeed> PriceFeeds { get; set; }

        [JsonProperty("tokenPrices")]
        public List<OracleTokenPrice> TokenPrices { get; set; }
    }
}
