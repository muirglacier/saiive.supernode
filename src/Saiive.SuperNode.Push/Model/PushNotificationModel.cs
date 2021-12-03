using Newtonsoft.Json;
using Saiive.Dobby.Api.Model;
using System.Collections.Generic;

namespace Saiive.SuperNode.Push.Model
{
    public class PushNotificationModelRegister
    {
        [JsonProperty("pushToken")]
        public string PushToken { get; set; }

        [JsonProperty("vaultId")]
        public string VaultId { get; set; }
    }

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
        public PushNotificationModelDatabase()
        {
            PushNotificationsInfo = new List<WebHookData>();
        }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; } = PushTokenPartitionKey;

        [JsonProperty("id")]
        public string Id => PushToken;

        [JsonProperty("dobbyUser")]
        public GetUserResponse DobbyUser { get; set; }

        [JsonProperty]
        public IList<WebHookData> PushNotificationsInfo { get; set; }
    }
}
