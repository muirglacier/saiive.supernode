using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class BlockProvider : BaseDeFiChainProvider, IBlockProvider
    {
        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetCurrentBlock(string network, int height)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/DFI/{network}/block/{height}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockModel>(data);
            return obj;

        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/DFI/{network}/block/tip");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockModel>(data);
            return obj;

        }
    }
}
