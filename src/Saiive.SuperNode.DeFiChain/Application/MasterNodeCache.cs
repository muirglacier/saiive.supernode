using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, Dictionary<string, List<Masternode>>> _cachedByOperatorList;
        private DateTime? _lastRefreshTime;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public MasterNodeCache(IConfiguration config, ILogger<MasterNodeCache> logger, MasterNodeHelper masterNodeHelper)
        {
            _logger = logger;
            _masterNodeHelper = masterNodeHelper;
            _cachedList = new Dictionary<string, List<Masternode>>();
            _cachedByOperatorList = new Dictionary<string, Dictionary<string, List<Masternode>>>();

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

                if(!_cachedByOperatorList.ContainsKey(network))
                {
                    _cachedByOperatorList.Add(network, new Dictionary<string, List<Masternode>>());
                }

                _cachedByOperatorList[network].Clear();
                
                foreach (var masternode in masterNodeList)
                {
                    if(!_cachedByOperatorList[network].ContainsKey(masternode.OwnerAuthAddress))
                    {
                        _cachedByOperatorList[network].Add(masternode.OwnerAuthAddress, new List<Masternode> { masternode });
                    }
                    else
                    {
                        _cachedByOperatorList[network][masternode.OwnerAuthAddress].Add(masternode);
                    }
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

        public async Task<bool> IsMasternodeStillAlive(string network, string ownerAddress, string txId)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (_cachedByOperatorList.Count == 0)
                {
                    await UpdateCachedList(network);
                }

                if (_cachedByOperatorList[network].ContainsKey(ownerAddress))
                {
                    var mn = _cachedByOperatorList[network][ownerAddress].FirstOrDefault(a => a.Id == txId);

                    if (mn == null)
                    {
                        throw new ArgumentException($"Masternode with id {txId} could not be found!");
                    }
                    return mn.State != "RESIGNED";
                }
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
            return false;
        }
    }
}
