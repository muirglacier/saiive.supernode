using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class BitcorePriceFeeds
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class BitcoreRawPrice
    {
        public string Key => $"{PriceFeed.Token}-{PriceFeed.Currency}";

        [JsonProperty("priceFeeds")]
        public BitcorePriceFeeds PriceFeed { get; set; }

        [JsonProperty("oracleid")]
        public string Oracleid { get; set; }

        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }

        [JsonProperty("rawprice")]
        public double Rawprice { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

}
