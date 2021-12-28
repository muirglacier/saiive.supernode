using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class Masternode
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("ownerAuthAddress")] 
        public string OwnerAuthAddress { get; set; }

        [JsonProperty("operatorAuthAddress")] 
        public string OperatorAuthAddress { get; set; }

        [JsonProperty("creationHeight")] 
        public int CreationHeight { get; set; }

        [JsonProperty("resignHeight")] 
        public int ResignHeight { get; set; }

        [JsonProperty("resignTx")] 
        public string ResignTx { get; set; }

        [JsonProperty("banHeight")] 
        public int BanHeight { get; set; }

        [JsonProperty("banTx")] 
        public string BanTx { get; set; }

        [JsonProperty("state")] 
        public string State { get; set; }

        [JsonProperty("mintedBlocks")] 
        public int MintedBlocks { get; set; }

        [JsonProperty("ownerIsMine")] 
        public bool OwnerIsMine { get; set; }

        [JsonProperty("operatorIsMine")] 
        public bool OperatorIsMine { get; set; }

        [JsonProperty("localMasternode")] 
        public bool LocalMasternode { get; set; }
    }
}
