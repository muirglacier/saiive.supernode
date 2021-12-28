using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class OraclesFunction : BaseFunction
    {
        public OraclesFunction(ILogger<OraclesFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("GetOracles")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Oracles" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<OracleData>), Description = "The OK response")]
        public async Task<IActionResult> GetSchemes(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/oracles")] HttpRequestMessage req,
          string network, string coin,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).OracleProvider.GetOralces(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetOraclePriceFeed")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Oracles" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "oracleId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "priceFeed", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<OraclePriceFeedData>), Description = "The OK response")]
        public async Task<IActionResult> GetCollaterals(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/oracles/{oracleId}/{priceFeed}/feed")] HttpRequestMessage req,
          string network, string coin, string oracleId, string priceFeed,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).OracleProvider.GetPriceFeedInfos(network, oracleId, priceFeed);

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
