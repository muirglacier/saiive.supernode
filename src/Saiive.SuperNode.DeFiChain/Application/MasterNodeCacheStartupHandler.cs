using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class MasterNodeCacheStartupHandler : IPeriodicJob
    {
        private readonly IMasterNodeCache _cache;
        private readonly ILogger<MasterNodeCacheStartupHandler> _logger;
        private readonly IConfiguration _config;

        public MasterNodeCacheStartupHandler(IMasterNodeCache cache, ILogger<MasterNodeCacheStartupHandler> logger, IConfiguration config)
        {
            _cache = cache;
            _logger = logger;
            _config = config;
        }

        private async Task UpdateCache()
        {
            try
            {
                _logger.LogInformation("Update masternode cache...");
                var mainnet = _cache.GetMasterNodes("mainnet");
                var testnet = _cache.GetMasterNodes("testnet");

                await Task.WhenAll(mainnet, testnet);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error starting up", e);
            }

            _logger.LogInformation("Update masternode cache...done");
        }

        public async Task Run()
        {
            await UpdateCache();
        }
    }
}
