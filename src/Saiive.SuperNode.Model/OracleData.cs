using System.Collections.Generic;
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

    public class OraclePriceFeedData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("oracleId")]
        public string OracleId { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("txid")]
        public string Txid { get; set; }
    }


    public class OracleData
    {
        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("id")]
        public string Oracleid { get; set; }

        [JsonProperty("priceFeeds")]
        public List<PriceFeed> PriceFeeds { get; set; }

        [JsonProperty("tokenPrices")]
        public List<OracleTokenPrice> TokenPrices { get; set; }
    }
}
