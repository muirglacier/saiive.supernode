using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Bitcoin.Helper;
using Saiive.SuperNode.Model;
using System;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class BlockProvider : BaseBitcoinProvider, IBlockProvider
    {
        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetBlockByHeightOrHash(string network, string hash)
        {
            var instance = GetInstance(network);
            
            var block = await instance.GetBlockByHeight(Convert.ToInt32(hash));
            return block.ToBlockModel(network);
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var instance = GetInstance(network);

            var stats = await instance.GetStats();
            var block = await GetBlockByHeightOrHash(network, stats.Height.ToString());

            return block;
        }
    }
}
