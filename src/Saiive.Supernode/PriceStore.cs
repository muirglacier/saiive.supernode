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
    public class PriceStore : IPriceStore
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl;

        private readonly Dictionary<string, Dictionary<string, ActivePrice>> _tokenStore =
            new Dictionary<string, Dictionary<string, ActivePrice>>();

        private readonly Dictionary<string, List<ActivePrice>> _tokenStoreRaw =
                    new Dictionary<string, List<ActivePrice>>();

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private DateTime _lastRefresh = DateTime.UtcNow.AddDays(-1);
        private const double _refreshIntervalInMinutes = 30.0;


        public PriceStore(IConfiguration config)
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

        public async Task<ActivePrice> GetPrice(string network, string tokenName)
        {
            if(tokenName == "DUSD-USD")
            {
                return null;
            }

            await _semaphoreSlim.WaitAsync();
            await CheckForRefresh(network);
            try
            {
                if (!_tokenStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }
                if (!_tokenStore[network].ContainsKey(tokenName))
                {
                    await LoadAll(network);
                }

              
                return _tokenStore[network][tokenName];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }

        private async Task LoadAll(string network)
        {
            var response = await _client.GetAsync($"{_apiUrl}/api/DFI/{network}/oracles/pricesraw");
            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var tokens =
            JsonConvert.DeserializeObject<List<BitcoreRawPrice>>(data);
            foreach (var token in tokens)
            {
                var converted = Convert(token);
                if (!_tokenStore.ContainsKey(network))
                {
                    _tokenStore.Add(network, new Dictionary<string, ActivePrice>());
                    
                    _tokenStoreRaw.Add(network, new List<ActivePrice>());
                }
                if (!_tokenStore[network].ContainsKey(token.Key))
                {
                    _tokenStore[network].Add(token.Key, converted);
                    
                    _tokenStoreRaw[network].Add(converted);
                }
            }
            _lastRefresh = DateTime.UtcNow;
        }

      

        public async Task<IList<ActivePrice>> GetAll(string network)
        {
            await _semaphoreSlim.WaitAsync(); 
            await CheckForRefresh(network);
            try
            {
                if (!_tokenStore.ContainsKey(network))
                {
                    await LoadAll(network);
                }

                return _tokenStoreRaw[network];
            }
            finally
            {
                _semaphoreSlim.Release(1);
            }
        }
    }
}
