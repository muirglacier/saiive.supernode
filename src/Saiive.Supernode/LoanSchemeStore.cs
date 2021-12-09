using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saiive.SuperNode.DeFiChain.Application;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Saiive.SuperNode
{
    public class LoanSchemeStore 
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl;

        private readonly Dictionary<string, Dictionary<string, LoanScheme>> _loanSchemeStore =
            new Dictionary<string, Dictionary<string, LoanScheme>>();

        private readonly Dictionary<string, List<LoanScheme>> _loanSchemeStoreRaw =
                    new Dictionary<string, List<LoanScheme>>();


        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private DateTime _lastRefresh = DateTime.UtcNow.AddDays(-1);
        private const double _refreshIntervalInMinutes = 180;


        public LoanSchemeStore(IConfiguration config)
        {
            _apiUrl = config["BITCORE_URL"];
        }

        private ActivePrice Convert(BitcoreRawPrice rawPrice)
        {
            return new ActivePrice
            {
                Id = rawPrice.Key,
                Key = rawPrice.Key,
                IsLive = rawPrice.State == "live",
                Active = new Active
                {
                    Amount = rawPrice.Rawprice.ToString(),
                    Weightage = rawPrice.Weightage
                }
            };
        }

        private async Task CheckForRefresh(string network)
        {
            if((DateTime.UtcNow - _lastRefresh).TotalMinutes > _refreshIntervalInMinutes)
            {
                await LoadAll(network);
            }
        }

        public async Task<LoanScheme> GetScheme(string network, string schemeId)
        {
            await _semaphoreSlim.WaitAsync();
            await CheckForRefresh(network);
            try
            {
                if (!_loanSchemeStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }
                if (!_loanSchemeStore[network].ContainsKey(schemeId))
                {
                    await LoadAll(network);
                }

              
                return _loanSchemeStore[network][schemeId];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }

        private async Task LoadAll(string network)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/DFI/{network}/loans/schemes");
            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var schemes =
            JsonConvert.DeserializeObject<List<LoanScheme>>(data);
            foreach (var scheme in schemes)
            {
                if (!_loanSchemeStore.ContainsKey(network))
                {
                    _loanSchemeStore.Add(network, new Dictionary<string, LoanScheme>());
                    _loanSchemeStoreRaw.Add(network, new List<LoanScheme>());
                }

                if (!_loanSchemeStore[network].ContainsKey(scheme.Id))
                {
                    _loanSchemeStore[network].Add(scheme.Id, scheme);
                    _loanSchemeStoreRaw[network].Add(scheme);
                }
            }
            _lastRefresh = DateTime.UtcNow;
        }

      

        public async Task<IList<LoanScheme>> GetAll(string network)
        {
            await _semaphoreSlim.WaitAsync(); 
            await CheckForRefresh(network);
            try
            {
                if (!_loanSchemeStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }

                return _loanSchemeStoreRaw[network];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }
    }
}
