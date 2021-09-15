using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction.Providers;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Saiive.SuperNode.DeFiChain.Providers
{
    internal class PoolPairProvider : BaseDeFiChainProvider, IPoolPairProvider
    {
        public PoolPairProvider(ILogger<PoolPairProvider> logger, IConfiguration config) : base(logger, config)
        {
        }

        public async Task<Dictionary<string, PoolPairModel>> GetPoolPair(string network, string poolId)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/poolpairs/{poolId}");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<Ocean.OceanDataEntity<Ocean.OceanPoolPairData>>(data);
            var ret = new Dictionary<string, PoolPairModel>();

            var poolPairModel = ConvertFromOceanModel(obj.Data);

            ret.Add(poolPairModel.ID, poolPairModel);


            return ret;
        }

        public async Task<Dictionary<string, PoolPairModel>> GetPoolPairs(string network)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/poolpairs");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<Ocean.OceanPoolPair>(data);


            var ret = new Dictionary<string, PoolPairModel>();

            foreach(var pair in obj.Data)
            {
                ret.Add(pair.Id, ConvertFromOceanModel(pair));
            }

            return ret;


        }

        public async Task<Dictionary<string, PoolPairModel>> GetPoolPairsBySymbolKey(string network)
        {
            var response = await _client.GetAsync($"{OceanUrl}/v0/{network}/poolpairs");


            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<Ocean.OceanPoolPair>(data);


            var ret = new Dictionary<string, PoolPairModel>();

            foreach (var pair in obj.Data)
            {
                ret.Add(pair.Symbol, ConvertFromOceanModel(pair));
            }

            return ret;
        }

        private PoolPairModel ConvertFromOceanModel(Ocean.OceanPoolPairData pair)
        {
            return new PoolPairModel
            {
                BlockCommissionA = Convert.ToDouble(pair.TokenA.BlockCommission, CultureInfo.InvariantCulture),
                BlockCommissionB = Convert.ToDouble(pair.TokenB.BlockCommission, CultureInfo.InvariantCulture),
                Commission = Convert.ToDouble(pair.Commission, CultureInfo.InvariantCulture),
                CreationHeight = pair.Creation.Height,
                CreationTx = pair.Creation.Tx,
                CustomRewards = pair.CustomRewards,
                ID = pair.Id,
                IdTokenA = pair.TokenA.Id,
                IdTokenB = pair.TokenB.Id,
                Name = pair.Name,
                OwnerAddress = pair.OwnerAddress,
                ReserveA = Convert.ToDouble(pair.TokenA.Reserve, CultureInfo.InvariantCulture),
                ReserveB = Convert.ToDouble(pair.TokenB.Reserve, CultureInfo.InvariantCulture),
                ReserveADivReserveB = Convert.ToDouble(pair.PriceRatio.Ab, CultureInfo.InvariantCulture),
                ReserveBDivReserveA = Convert.ToDouble(pair.PriceRatio.Ba, CultureInfo.InvariantCulture),
                RewardPct = Convert.ToDouble(pair.RewardPct, CultureInfo.InvariantCulture),
                Status = pair.Status,
                Symbol = pair.Symbol,
                TotalLiquidity = Convert.ToDouble(pair.TotalLiquidity.Token, CultureInfo.InvariantCulture),
                TotalLiquidityRaw = Convert.ToDouble(pair.TotalLiquidity.Token, CultureInfo.InvariantCulture) * 100000000,
                TotalLiquidityUsd = Convert.ToDouble(pair.TotalLiquidity.Usd, CultureInfo.InvariantCulture),
                TradeEnabled = pair.TradeEnabled
            };
        }
    }
}
