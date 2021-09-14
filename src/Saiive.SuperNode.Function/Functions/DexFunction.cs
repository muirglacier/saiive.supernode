using System;
using System.Collections.Generic;
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

namespace Saiive.SuperNode.Function.Functions
{
    public class DexFunction : BaseFunction
    {
        public DexFunction(ILogger<DexFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("Testpoolswap")]
        [OpenApiOperation(operationId: "Testpoolswap", tags: new[] { "DEX" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(TestPoolSwapBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<YieldFramingModel>), Description = "The OK response")]
        public async Task<IActionResult> Testpoolswap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/dex/testpoolswap")] TestPoolSwapBodyRequest req,
            string network, string coin,
            ILogger log)
        {

            //TODO CHANGE URGENT!
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{String.Format(ApiUrl, network)}/api/{coin}/{network}/lp/testpoolswap", httpContent);

            var data = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();

                var result = new TestPoolSwapModel();
                result.Result = data;

                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e} ({data})");
                return new BadRequestObjectResult(new ErrorModel($"{e.Message} ({data})"));
            }
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

    }
}

