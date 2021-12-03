using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Push.Model
{
    public class PushNotificationModel
    {

        public PushNotificationModel()
        {
            VaultIds = new List<string>();
        }


        [JsonProperty("pushToken")]
        public string PushToken { get; set; }

        [JsonProperty("vaultIds")]
        public IList<string> VaultIds { get; set; }
    }

    internal class PushNotificationModelDatabase : PushNotificationModel
    {

        public const string PushTokenPartitionKey = "pushToken";


        [JsonProperty("partitionKey")]
        public string PartitionKey { get; } = PushTokenPartitionKey;

        [JsonProperty("id")]
        public string Id => PushToken;
    }
}
