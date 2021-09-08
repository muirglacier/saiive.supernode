using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Saiive.SuperNode.DeFiChain.Application
{
    internal class MasterNodeCacheStartupHandler : IHostedService
    {
        private readonly IMasterNodeCache _cache;
        private readonly ILogger<MasterNodeCacheStartupHandler> _logger;
        private readonly IConfiguration _config;
        private Timer _timer;

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _timer = new Timer(async state =>
            {
                await UpdateCache();
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
