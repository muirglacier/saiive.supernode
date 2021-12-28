using System.Collections.Generic;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class YieldFramingModelRequest
    {
        [JsonProperty("pools")]
        public List<YieldFramingModel> Pools { get; set; }
    }
}
