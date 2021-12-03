using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Saiive.SuperNode.Push.Model
{
    public class VaultIdPushTokenMapping
    {
        public const string VaultMapPartitionKey = "vaultMap";

        public VaultIdPushTokenMapping()
        {
            PushTokenList = new List<string>();
        }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; } = VaultMapPartitionKey;

        [JsonProperty("id")]
        public string Id => VaultId;

        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("pushTokenList")]
        public List<string> PushTokenList  { get; set; }
    }
}
