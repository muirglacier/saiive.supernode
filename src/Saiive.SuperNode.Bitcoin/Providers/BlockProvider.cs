using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System.Collections.Generic;
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
            var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block/{hash}");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockModel>(data);

            return obj;
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block/tip");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<BlockModel>(data);
            return obj;
        }

            public async  Task<List<BlockModel>> GetLatestBlocks(string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block/latest?limit=5");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<List<BlockModel>>(data);
            return obj;
        }
    }
}
