using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class CreateNotificationTriggerResponse : ApiResponse
    {
        [JsonProperty("trigger")]
        public Trigger? Trigger { get; set; }
    }

    public class Gateway
    {
        [JsonProperty("gatewayId")]
        public int GatewayId { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }
    }

    public class Trigger
    {
        public Trigger()
        {
            Gateways = new List<Gateway>();
        }

        [JsonProperty("triggerId")]
        public int TriggerId { get; set; }

        [JsonProperty("ratio")]
        public int Ratio { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("gateways")]
        public List<Gateway> Gateways { get; set; }
    }



}
