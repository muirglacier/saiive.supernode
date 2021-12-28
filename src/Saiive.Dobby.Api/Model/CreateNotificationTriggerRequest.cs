using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class CreateNotificationTriggerRequest
    {
        public CreateNotificationTriggerRequest()
        {
            Gateways = new List<int>();
        }

        [JsonProperty("vaultId")]
        public string? VaultId { get; set; }

        [JsonProperty("ratio")]
        public int Ratio { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("gateways")]
        public List<int> Gateways { get; set; }

    }
}
