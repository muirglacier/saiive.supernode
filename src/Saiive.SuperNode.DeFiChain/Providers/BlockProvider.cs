using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
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
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/blocks/{height}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<OceanBlock>(data);
            return ConvertOceanModel(obj);

        }

        private BlockModel ConvertOceanModel(OceanBlock oceanBlock)
        {
            return new BlockModel
            {
                Hash = oceanBlock.Data.Hash,
                Height = oceanBlock.Data.Height,
                MerkleRoot = oceanBlock.Data.Merkleroot,
                PreviousBlockHash = oceanBlock.Data.PreviousHash,
                Size = oceanBlock.Data.Size,
                TransactionCount = oceanBlock.Data.TransactionCount,
                Time = UnixTimeToDateTime(oceanBlock.Data.Time).ToString(),
                Id = oceanBlock.Data.Id
            };
        }
        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var stats = await _client.GetAsync($"{OceanUrl}/v0/{network}/stats");
            var statsData = await stats.Content.ReadAsStringAsync();

            var statsObj = JsonConvert.DeserializeObject<OceanStats>(statsData);

            return await GetCurrentBlock(network, statsObj.Data.Count.Blocks);

        }
    }
}
