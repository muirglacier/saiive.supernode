using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
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

        public async Task<List<StockPrice>> GetPrices(string network)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<StockPrice>($"{OceanUrl}/{ApiVersion}/{network}/prices");
            return oceanData;
        }
    }
}
