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
            var oceanData = await Helper.LoadAllFromPagedRequest<OracleData>($"{OceanUrl}/{ApiVersion}/{network}/oracles");
            return oceanData;
        }

        public async Task<IList<OraclePriceFeedData>> GetPriceFeedInfos(string network, string oracleId, string priceFeed)
        {
            var oceanData = await Helper.LoadAllFromPagedRequest<OraclePriceFeedData>($"{OceanUrl}/{ApiVersion}/{network}/oracles/{oracleId}/{priceFeed}/feed", 100);
            return oceanData;
        }
    }
}
