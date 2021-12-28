using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    public class ApiResponse
    {
        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
