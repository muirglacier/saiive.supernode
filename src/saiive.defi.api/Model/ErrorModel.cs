using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class ErrorModel
    {
        public ErrorModel(string error)
        {
            Error = error;
        }
        
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
