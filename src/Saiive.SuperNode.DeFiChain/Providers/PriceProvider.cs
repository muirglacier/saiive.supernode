using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class PriceProvider : BaseDeFiChainProvider, IPriceProvider
    {
        public PriceProvider(ILogger<PriceProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<List<PriceFeedInfo>> GetFeed(string network, string token, string currency)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<PriceFeedInfo>($"{OceanUrl}/{ApiVersion}/{network}/prices/{token}-{currency}/feed");
            return oceanData;
        }

        public async Task<List<PriceFeedInfo>> GetFeedWithInterval(string network, string token, string currency, PriceFeedInterval interval)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<PriceFeedInfo>($"{OceanUrl}/{ApiVersion}/{network}/prices/{token}-{currency}/feed/interval/{(int)interval}");
            return oceanData;
        }

        public async Task<List<OracleInfo>> GetOracles(string network, string token, string currency)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<OracleInfo>($"{OceanUrl}/{ApiVersion}/{network}/prices/{token}-{currency}/oracles");
            return oceanData;
        }

        public async Task<StockPrice> GetPrice(string network, string token, string currency)
        {
            var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/prices/{token}-{currency}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<OceanDataEntity<StockPrice>>(data);
            return json.Data;
            
        }

        public async Task<List<StockPrice>> GetPrices(string network)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<StockPrice>($"{OceanUrl}/{ApiVersion}/{network}/prices");
            return oceanData;
        }
    }
}
