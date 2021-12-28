using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    internal class CreateUserRequest
    {
        [JsonProperty("language")]
        public string? Language { get; set; }
        
        [JsonProperty("theme")]
        public string? Theme { get; set; }
    }
}
