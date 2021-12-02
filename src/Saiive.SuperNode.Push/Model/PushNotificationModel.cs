using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Push.Model
{
    public class PushNotificationModel
    {
        public const string PushTokenPartitionKey = "pushToken";

        public PushNotificationModel()
        {
            VaultIds = new List<string>();
        }

        [JsonIgnore]
        public string PartitionKey { get; } = PushTokenPartitionKey;

        [JsonProperty("pushToken")]
        public string PushToken { get; set; }

        [JsonProperty("vaultIds")]
        public IList<string> VaultIds { get; set; }
    }
}
