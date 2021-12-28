using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class GetNotificationRequestResponse
    {
        public GetNotificationRequestResponse()
        {
            Data = new List<Trigger>();
        }

        [JsonProperty("data")]
        public IList<Trigger> Data { get; set; }
    }
}
