using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class NotificationGatewayResponse
    {
        [JsonProperty("gatewayId")]
        public int GatewayId { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("gatewayId")]
        public string? Value { get; set; }
    }

    public class NotificationGatewayResponseList
    {
        public NotificationGatewayResponseList()
        {
            Data = new List<NotificationGatewayResponse>();
        }

        [JsonProperty("data")]
        public IList<NotificationGatewayResponse> Data { get; set; }
    }
}
