using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class StatsFunction : BaseFunction
    {
        public StatsFunction(ILogger<StatsFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("Stats")]
        [OpenApiOperation(operationId: "StatsFunction", tags: new[] { "StatsFunction" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StatsModel), Description = "The OK response")]
        public async Task<IActionResult> Stats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/stats")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).StatsProvider.GetStats(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
        [FunctionName("EnabledChains")]
        [OpenApiOperation(operationId: "StatsFunction", tags: new[] { "Status" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<EnabledChain>), Description = "The OK response")]
        public async Task<IActionResult> EnabledChains(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/status/enabled-chains")] HttpRequestMessage req,
            ILogger log)
        {

            try
            {
                await Task.CompletedTask;
                var ret = new List<EnabledChain>();

                foreach(var x in ChainProviderCollection)
                {
                    ret.Add(new EnabledChain
                    {
                        Chain = x.Key,
                        Network = "mainnet"
                    });
                    ret.Add(new EnabledChain
                    {
                        Chain = x.Key,
                        Network = "testnet"
                    });
                }

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


    }
}

