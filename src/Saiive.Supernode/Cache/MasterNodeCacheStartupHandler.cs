using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Saiive.SuperNode.Cache
{
    public class MasterNodeCacheStartupHandler : IHostedService
    {
        private readonly IMasterNodeCache _cache;
        private readonly ILogger<MasterNodeCacheStartupHandler> _logger;
        private Timer _timer;

        public MasterNodeCacheStartupHandler(IMasterNodeCache cache, ILogger<MasterNodeCacheStartupHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private async Task UpdateCache()
        {
            _logger.LogInformation("Update masternode cache...");
            var mainnet = _cache.GetMasterNodes("mainnet", "DFI");
            var testnet = _cache.GetMasterNodes("testnet", "DFI");

            await Task.WhenAll(mainnet, testnet);

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
