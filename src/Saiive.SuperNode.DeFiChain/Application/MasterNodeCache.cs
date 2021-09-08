using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.DeFiChain.Providers;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class MasterNodeCache : IMasterNodeCache
    {
        private readonly ILogger _logger;
        private readonly MasterNodeHelper _masterNodeHelper;
        private readonly Dictionary<string, List<Masternode>> _cachedList;
        private DateTime? _lastRefreshTime;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public MasterNodeCache(IConfiguration config, ILogger<MasterNodeCache> logger, MasterNodeHelper masterNodeHelper)
        {
            _logger = logger;
            _masterNodeHelper = masterNodeHelper;
            _cachedList = new Dictionary<string, List<Masternode>>();

        }


        private async Task UpdateCachedList(string network)
        {
           
            try
            {
                var masterNodeList = await _masterNodeHelper.LoadAll(network);


                _lastRefreshTime = DateTime.UtcNow;

                if (!_cachedList.ContainsKey(network))
                {
                    _cachedList.Add(network, masterNodeList);
                }
                else
                {
                    _cachedList[network] = masterNodeList;
                }


            }
            catch (Exception e)
            {
                _logger.LogError($"{e}");
            }
        }

        public async Task<List<Masternode>> GetMasterNodes(string network)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!_cachedList.ContainsKey(network) ||
                    _cachedList[network].Count == 0 ||
                    _lastRefreshTime == null ||
                    DateTime.UtcNow - _lastRefreshTime.Value > TimeSpan.FromDays(1))
                {
                    await UpdateCachedList(network);
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
