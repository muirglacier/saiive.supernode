using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Cache
{
    public class MasterNodeCache : IMasterNodeCache
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, List<Masternode>> _cachedList;
        private DateTime? _lastRefreshTime;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private readonly HttpClient _client;
        protected readonly string ApiUrl;

        public MasterNodeCache(IConfiguration config, ILogger<MasterNodeCache> logger)
        {
            _logger = logger;
            _cachedList = new Dictionary<string, List<Masternode>>();

            ApiUrl = config["BITCORE_URL"];

            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);
        }


        private async Task UpdateCachedList(string network, string coin)
        {
            
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/masternodes/list");

            try
            {
                var data = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();


                var masterNodeList = JsonConvert.DeserializeObject<Dictionary<string, Masternode>>(data);

                var ret = new List<Masternode>();

                foreach (var masternode in masterNodeList)
                {
                    masternode.Value.Id = masternode.Key;
                    ret.Add(masternode.Value);
                }

                _lastRefreshTime = DateTime.UtcNow;

                if (!_cachedList.ContainsKey(network))
                {
                    _cachedList.Add(network, ret);
                }
                else
                {
                    _cachedList[network] = ret;
                }


            }
            catch (Exception e)
            {
                _logger.LogError($"{e}");
            }
        }

        public async Task<List<Masternode>> GetMasterNodes(string network, string coin)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!_cachedList.ContainsKey(network) ||
                    _cachedList[network].Count == 0 ||
                    _lastRefreshTime == null ||
                    (DateTime.UtcNow - _lastRefreshTime.Value) > TimeSpan.FromDays(1))
                {
                    await UpdateCachedList(network, coin);
                }

                return _cachedList[network];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }
    }
}
