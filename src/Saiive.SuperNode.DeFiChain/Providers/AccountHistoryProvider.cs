using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class AccountHistoryProvider : BaseDeFiChainProvider, IAccountHistoryProvider
    {
        public AccountHistoryProvider(ILogger<AccountHistoryProvider> logger, IConfiguration config) : base(logger, config)
        {

        }

        private async Task<List<AccountHistory>> GetAccountHistoryInternal(string coin, string network, string address, string token, string limit, string maxBlockHeight, bool noRewards)
        {

            throw new NotImplementedException();

            //string query = $"{ApiUrl}/api/{coin}/{network}/lp/listaccounthistory/{address}/{System.Web.HttpUtility.UrlEncode(token)}";

            //var dict = new Dictionary<string, string>();

            //if (!String.IsNullOrEmpty(maxBlockHeight))
            //{
            //    dict.Add("maxBlockHeight", maxBlockHeight);
            //}

            //if (!String.IsNullOrEmpty(limit))
            //{
            //    dict.Add("limit", limit);
            //}

            //dict.Add("no_rewards", noRewards.ToString());

            //var response = await _client.GetAsync(QueryHelpers.AddQueryString(query, dict));

            //response.EnsureSuccessStatusCode();

            //var data = await response.Content.ReadAsStringAsync();

            //var obj = JsonConvert.DeserializeObject<List<AccountHistory>>(data);

            //return obj;
        }

        public async Task<List<AccountHistory>> GetAccountHistory(string network, string address, string token, string limit, string maxBlockHeight, bool no_rewards)
        {
            var history = await GetAccountHistoryInternal("DFI", network, address, token, limit, maxBlockHeight, no_rewards);
            return history;
        }

        public async Task<List<AccountHistory>> GetTotalBalance(string network, string token, string limit, string maxBlockHeight, bool no_rewards, AddressesBodyRequest addresses)
        {
            var retHistory = new List<AccountHistory>();

            foreach (var address in addresses.Addresses)
            {
                var histories = await GetAccountHistoryInternal("DFI", network, address, token, limit, maxBlockHeight, no_rewards);

                retHistory.AddRange(histories);
            }
            return retHistory;
        }
    }
}
