using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class YieldFramingModelRequest
    {
        [JsonProperty("pools")]
        public List<YieldFramingModel> pools { get; set; }
    }
}
