using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class GetNotificationGatewaysResponse
    {
        public GetNotificationGatewaysResponse()
        {
            Data = new List<Gateway>();
        }

        [JsonProperty("data")]
        public IList<Gateway> Data { get; set; }
    }
}
