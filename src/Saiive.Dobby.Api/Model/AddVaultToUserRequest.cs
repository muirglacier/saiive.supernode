using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    public class AddVaultToUserRequest
    {
        [JsonProperty("vaultID")]
        public string? VaultId { get; set; }
    }
}
