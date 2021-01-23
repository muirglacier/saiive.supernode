using System.Collections.Generic;
using Newtonsoft.Json;

namespace saiive.defi.api.Model
{
    public class Account
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("accounts")]
        public IList<AccountModel> Accounts { get; set; }
    }
}
