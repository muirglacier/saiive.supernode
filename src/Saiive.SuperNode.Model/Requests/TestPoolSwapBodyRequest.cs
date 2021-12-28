﻿using Newtonsoft.Json;

namespace Saiive.SuperNode.Model.Requests
{
    public class TestPoolSwapBodyRequest
    {
        [JsonProperty("from")]
        public string From{ get; set; }

        [JsonProperty("tokenFrom")]
        public string TokenFrom { get; set; }

        [JsonProperty("amountFrom")]
        public double AmountFrom { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("tokenTo")]
        public string TokenTo { get; set; }

        [JsonProperty("maxPrice")]
        public double MaxPrice { get; set; }
    }
}
