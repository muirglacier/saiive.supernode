using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    internal class CreateNotificationGatewayRequest
    {
        [JsonProperty("type")]
        public string? Type { get; set; } = "webhook";

        [JsonProperty("value")]
        public string? Value { get; set; }
    }
}
