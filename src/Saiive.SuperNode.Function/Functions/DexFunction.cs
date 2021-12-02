using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class DexFunction : BaseFunction
    {
        public DexFunction(ILogger<DexFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("ListPoolPairs")]
        [OpenApiOperation(operationId: "ListPoolPairs", tags: new[] { "DEX" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Masternode>), Description = "The OK response")]
        public async Task<IActionResult> ListPoolPairs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/listpoolpairs")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).PoolPairProvider.GetPoolPairs(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("GetPoolPair")]
        [OpenApiOperation(operationId: "GetPoolPair", tags: new[] { "DEX" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "poolId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Masternode>), Description = "The OK response")]
        public async Task<IActionResult> GetPoolPair(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/getpoolpair/{poolId}")] HttpRequestMessage req,
            string network, string coin, string poolId,
            ILogger log)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).PoolPairProvider.GetPoolPair(network, poolId);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("ListMinePoolShares")]
        [OpenApiOperation(operationId: "ListMinePoolShares", tags: new[] { "DEX" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<string, PoolShareModel>), Description = "The OK response")]
        public async Task<IActionResult> ListMinePoolShares(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/listminepoolshares")] AddressesBodyRequest req,
            string network, string coin,
            ILogger log)
        {

            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var obj = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAccount(network, req);
                var poolPairs = await ChainProviderCollection.GetInstance(coin).PoolPairProvider.GetPoolPairsBySymbolKey(network);
                var ret = new Dictionary<string, PoolShareModel>();

                foreach(var p in obj)
                {
                    foreach(var addr in p.Accounts.Where(a => a.IsLPS))
                    {
                        var poolPair = poolPairs[addr.SymbolKey];

                        var poolShare = new PoolShareModel
                        {
                            Key = $"{poolPair.ID}@{addr.Address}",
                            Amount = addr.Balance / 100000000,
                            Owner = addr.Address,
                            PoolID = poolPair.ID,
                            TotalLiquidity = poolPair.TotalLiquidity,
                            Percent = (addr.Balance* 100) / poolPair.TotalLiquidityRaw
                        };

                        if (!ret.ContainsKey(poolShare.Key))
                        {
                            ret.Add(poolShare.Key, poolShare);
                        }
                    }
                }

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("ListPoolShares")]
        [OpenApiOperation(operationId: "ListPoolShares", tags: new[] { "DEX" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<PoolShareModel>), Description = "The OK response")]
        public async Task<IActionResult> ListPoolShares(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/listpoolshares")] AddressesBodyRequest req,
            string network, string coin,
            ILogger log)
        {
            req.Addresses = req.Addresses.Distinct().ToList();
            await Task.CompletedTask;
            return new OkObjectResult(new List<PoolShareModel>());
        }

        }
}

