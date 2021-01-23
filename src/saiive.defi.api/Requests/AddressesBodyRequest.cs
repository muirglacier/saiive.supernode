using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Requests
{
    public class AddressesBodyRequest
    {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }
    }
}
