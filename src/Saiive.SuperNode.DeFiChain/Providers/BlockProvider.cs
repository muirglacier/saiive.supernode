using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.DeFiChain.Ocean;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class BlockProvider : BaseDeFiChainProvider, IBlockProvider
    {
        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetBlockByHeightOrHash(string network, string hash)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/blocks/{hash}");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<OceanBlock>(data);
            return ConvertOceanModel(obj);

        }


        public async Task<List<TransactionModel>> GetTransactionForBlock(string network, string hash)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/blocks/{hash}/transactions");

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<OceanDataEntity<List<OceanTransactionDetailData>>>(data);
            return ConvertOceanModel(obj);

        }

        private List<TransactionModel> ConvertOceanModel(OceanDataEntity<List<OceanTransactionDetailData>> data)
        {
            var res = new List<TransactionModel>();

            foreach(var d in data.Data)
            {
                var tx = new TransactionModel
                {
                    Id = d.Id
                };
                res.Add(tx);
            }
            

            return res;
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
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            var stats = await _client.GetAsync($"{OceanUrl}/v0/{network}/stats");
            var statsData = await stats.Content.ReadAsStringAsync();

            var statsObj = JsonConvert.DeserializeObject<OceanStats>(statsData);

            return await GetBlockByHeightOrHash(network, statsObj.Data.Count.Blocks.ToString());

        }
    }
}
