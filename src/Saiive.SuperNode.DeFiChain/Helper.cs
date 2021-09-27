using Newtonsoft.Json;
using Saiive.SuperNode.DeFiChain.Ocean;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain
{
    public static class Helper
    {
        private readonly static HttpClient _client;

        static Helper()
        {

            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);
        }

        public static NBitcoin.Network GetNBitcoinNetwork(string network)
        {
            if(network.ToLower() == "mainnet")
            {
                return Saiive.DeFiChain.Sharp.Parser.NBitcoin.DeFiChain.Instance.Mainnet;
            }
            else if(network.ToLower() == "testnet")
            {
                return Saiive.DeFiChain.Sharp.Parser.NBitcoin.DeFiChain.Instance.Testnet;
            }
            throw new ArgumentException($"{network} is invalid!");
        }

        public static async Task<List<T>> LoadAllFromPagedRequest<T>(string url)
        {
            var q = new Queue<string>();
            q.Enqueue(null);

            var ret = new List<T>();

            while (q.Count > 0)
            {
                var q2 = q.Dequeue();
                var data = await LoadPage<T>(url, q2);

                ret.AddRange(data.Data);

                if (data.Page != null && !String.IsNullOrEmpty(data.Page.Next))
                {
                    q.Enqueue(data.Page.Next);
                }
            }
            return ret;

        }

        private static async Task<OceanDataEntity<List<T>>> LoadPage<T>(string url, string nextPage)
        {
            if (!String.IsNullOrEmpty(nextPage))
            {
                url += $"?next={nextPage}";
            }

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OceanDataEntity<List<T>>>(data);
        }
    }
}
