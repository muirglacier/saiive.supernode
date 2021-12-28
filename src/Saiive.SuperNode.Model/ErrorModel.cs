using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
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
