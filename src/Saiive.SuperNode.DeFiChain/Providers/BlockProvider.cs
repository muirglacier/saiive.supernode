﻿using Microsoft.Extensions.Configuration;
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

        public BlockProvider(ILogger<BlockProvider> logger, IConfiguration config ) : base(logger, config)
        {
        }

        public async Task<BlockModel> GetBlockByHeightOrHash(string network, string hash)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/block/{hash}", async () =>
            {
                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/blocks/{hash}");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<OceanBlock>(data);
                return ConvertOceanModel(obj.Data);
            }, null);

        }


        public async Task<List<TransactionModel>> GetTransactionForBlock(string network, string hash)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/tx/block/{hash}", async () =>
            {
                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/blocks/{hash}/transactions");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var tip = await GetCurrentHeight(network);

                var obj = JsonConvert.DeserializeObject<OceanDataEntity<List<OceanTransactionDetailData>>>(data);
                return ConvertOceanModel(obj, tip);
            }, null);

        }

        private List<TransactionModel> ConvertOceanModel(OceanDataEntity<List<OceanTransactionDetailData>> data, BlockModel tip)
        {
            var res = new List<TransactionModel>();

            foreach(var d in data.Data)
            {
                var tx = new TransactionModel
                {
                    Id = d.Id,
                    BlockTime = UnixTimeToDateTime(d.Block.Time),
                    Confirmations = tip.Height - d.Block.Height,
                    BlockHeight = d.Block.Height

                };
                res.Add(tx);
            }
            

            return res;
        }

        private BlockModel ConvertOceanModel(OceanBlockData oceanBlock)
        {
            return new BlockModel
            {
                Hash = oceanBlock.Hash,
                Height = oceanBlock.Height,
                MerkleRoot = oceanBlock.Merkleroot,
                PreviousBlockHash = oceanBlock.PreviousHash,
                Size = oceanBlock.Size,
                TransactionCount = oceanBlock.TransactionCount,
                Time = UnixTimeToDateTime(oceanBlock.Time).ToString("o"),
                Id = oceanBlock.Id
            };
        }
     

        public async Task<BlockModel> GetCurrentHeight(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/block/tip", async () =>
            {
                var stats = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/stats");
                var statsData = await stats.Content.ReadAsStringAsync();

                var statsObj = JsonConvert.DeserializeObject<OceanStats>(statsData);

                return await GetBlockByHeightOrHash(network, statsObj.Data.Count.Blocks.ToString());
            }, null);

        }

        public async Task<List<BlockModel>> GetLatestBlocks(string network)
        {
            return await RunWithFallbackProvider($"api/v1/{network}/DFI/block/tip", async () =>
            {
                var response = await _client.GetAsync($"{OceanUrl}/{ApiVersion}/{network}/blocks");

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<OceanBlockList>(data);
                var ret = new List<BlockModel>();

                foreach (var o in obj.Data)
                {
                    ret.Add(ConvertOceanModel(o));
                }

                return ret;
            }, (b) =>
            {
                var block = JsonConvert.DeserializeObject<BlockModel>(b);

                return new List<BlockModel>() { block };
            });
        }
    }
}
