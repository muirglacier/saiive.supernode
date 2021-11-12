using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.DeFiChain.Providers;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class LoanVaultCache : ILoanVaultCache, IPeriodicJob
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly LoanProvider _loanProvider;
        private Dictionary<string, Dictionary<string, IList<LoanVault>>> _loanVaultCache = new Dictionary<string, Dictionary<string, IList<LoanVault>>>();
        private DateTime? _lastRefreshTime;

        public LoanVaultCache(LoanProvider loanProvider)
        {
            _loanProvider = loanProvider;
        }

        public async Task<IList<LoanVault>> GetLoanVaultsForAddress(string network, string address)
        {
            if (!_loanVaultCache.ContainsKey(network) ||
                    _loanVaultCache[network].Count == 0 ||
                    _lastRefreshTime == null ||
                    DateTime.UtcNow - _lastRefreshTime.Value > TimeSpan.FromMinutes(15))
            {
                await UpdateCache(network);
            }

            return await GetLoanVaultsForAddresses(network, new List<string>() { address });
        }

        public async Task<IList<LoanVault>> GetLoanVaultsForAddresses(string network, IList<string> addresses)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var ret = new List<LoanVault>();

                foreach (var address in addresses)
                {
                    if (_loanVaultCache.ContainsKey(network) && _loanVaultCache[network].ContainsKey(address))
                    {
                        ret.AddRange(_loanVaultCache[network][address]);
                    }
                }
                return ret;
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }

        private async Task UpdateCache(string network)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var loanVaults = await _loanProvider.GetLoanVaults(network);

                _lastRefreshTime = DateTime.UtcNow;

                if (!_loanVaultCache.ContainsKey(network))
                {
                    _loanVaultCache.Add(network, new Dictionary<string, IList<LoanVault>>());
                }

                _loanVaultCache[network].Clear();
                foreach (var loanVault in loanVaults)
                {
                    if(!_loanVaultCache[network].ContainsKey(loanVault.OwnerAddress))
                    {
                        _loanVaultCache[network].Add(loanVault.OwnerAddress, new List<LoanVault>());
                    }
                    _loanVaultCache[network][loanVault.OwnerAddress].Add(loanVault);
                }
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }

        public async Task Run()
        {
            await Task.WhenAll(new List<Task> { UpdateCache("mainnet"), UpdateCache("testnet") });
        }
    }
}
