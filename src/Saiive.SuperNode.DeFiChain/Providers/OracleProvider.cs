using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class OracleProvider : BaseDeFiChainProvider, IOracleProvider
    {
        public OracleProvider(ILogger<ILoanProvider> logger, IConfiguration config) : base(logger, config)
        {
        }


        public async Task<IList<OracleData>> GetOralces(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/oracles/oracles", network, async () =>
            {
                var oceanData = await Helper.LoadAllFromPagedRequest<OracleData>($"{OceanUrl}/{ApiVersion}/{network}/oracles");
                return oceanData;
            }, null);
        }

        public async Task<IList<OraclePriceFeedData>> GetPriceFeedInfos(string network, string oracleId, string priceFeed)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/oracle/prices", network, async () =>
            {
                var oceanData = await Helper.LoadAllFromPagedRequest<OraclePriceFeedData>($"{OceanUrl}/{ApiVersion}/{network}/oracles/{oracleId}/{priceFeed}/feed", 100);
                return oceanData;
            }, null);
        }
    }
}
