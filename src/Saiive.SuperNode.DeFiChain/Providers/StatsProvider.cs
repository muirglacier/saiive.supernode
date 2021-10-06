using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class StatsProvider : BaseDeFiChainProvider, IStatsProvider
    {
        public StatsProvider(ILogger<StatsProvider> logger, IConfiguration config, BlockProvider blockProvider) : base(logger, config)
        {
            BlockProvider = blockProvider;
        }

        public IBlockProvider BlockProvider { get; }

        public async Task<StatsModel> GetStats(string network)
        {
            var stats = await _client.GetAsync($"{OceanUrl}/v0/{network}/stats");
            var statsData = await stats.Content.ReadAsStringAsync();

            var statsObj = JsonConvert.DeserializeObject<OceanStats>(statsData);
            var latestBlock = await BlockProvider.GetCurrentHeight(network);


            return new StatsModel
            {
                Chain = network,
                BlockHeight = latestBlock.Height,
                BestBlockHash = latestBlock.Hash,
                BurnInfo = new BurnInfo
                {
                    Address = "8defichainBurnAddressXXXXXXXdRQkSm",
                    Amount = statsObj.Data.Burned.Total.ToString(),
                    Emissionburn = statsObj.Data.Burned.Emission.ToString(),
                    Feeburn = statsObj.Data.Burned.Fee
                }
            };

            
        }
    }
}
