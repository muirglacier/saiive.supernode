using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saiive.SuperNode.Model
{
    public class Oracles
    {
        [JsonProperty("active")]
        public int Active { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class Aggregated
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("oracles")]
        public Oracles Oracles { get; set; }
    }
    public class Block
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("medianTime")]
        public int MedianTime { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }
    }

    public class Price
    {
        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("aggregated")]
        public Aggregated Aggregated { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }
    }

    public class PriceFeedInfo
    {
        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("aggregated")]
        public Aggregated Aggregated { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }
    }

    public class OracleInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("oracleId")]
        public string OracleId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("feed")]
        public PriceFeed Feed { get; set; }
    }




    public class StockPrice
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }
    }
}
