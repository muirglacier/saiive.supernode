using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Bitcoin.Helper;
using Saiive.SuperNode.Model;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class BlockProvider : BaseBitcoinProvider, IBlockProvider
    {
        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetCurrentBlock(string network, int height)
        {
            var instance = GetInstance(network);

            var block = await instance.GetBlockByHeight(height);
            return block.ToBlockModel(network);
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var instance = GetInstance(network);

            var stats = await instance.GetStats();
            var block = await GetCurrentBlock(network, stats.Height);

            return block;
        }
    }
}
