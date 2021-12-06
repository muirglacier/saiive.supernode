using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saiive.SuperNode.Bitcoin.Helper;

namespace Saiive.SuperNode.Bitcoin.Providers
{
    internal class BlockProvider : BaseBitcoinProvider, IBlockProvider
    {
        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetBlockByHeightOrHash(string network, string hash)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block/{hash}");


                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BlockModel>(data);

                return obj;
            }
            catch
            {
                return await GetBlockByHeightOrHashCypher(network, hash);
            }
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block/tip");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<BlockModel>(data);
                return obj;
            }
            catch
            {
                return await GetCurrentHeightCypher(network);
            }
        }

        public async Task<List<BlockModel>> GetLatestBlocks(string network)
        {
            try
            {
                var response = await _client.GetAsync($"{ApiUrl}/api/BTC/{network}/block?limit=5");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<List<BlockModel>>(data);
                return obj;
            }
            catch
            {
                return await GetLatestBlocksCypher(network);
            }
        }


        public async Task<BlockModel> GetBlockByHeightOrHashCypher(string network, string hash)
        {
            var instance = GetInstance(network);

            var block = await instance.GetBlockByHeight(Convert.ToInt32(hash));
            return block.ToBlockModel(network);
        }

        public async Task<BlockModel> GetCurrentHeightCypher(string network)
        {
            var instance = GetInstance(network);

            var stats = await instance.GetStats();
            var block = await GetBlockByHeightOrHash(network, stats.Height.ToString());

            return block;
        }

        public async Task<List<BlockModel>> GetLatestBlocksCypher(string network)
        {
            var instance = GetInstance(network);

            await Task.CompletedTask;
            return new List<BlockModel>();
        }
    }
}
