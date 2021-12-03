using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    public class AddVaultToUserRequest
    {
        [JsonProperty("vaultId")]
        public string? VaultId { get; set; }
    }
}
