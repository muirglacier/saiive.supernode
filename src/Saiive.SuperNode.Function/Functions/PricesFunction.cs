using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Function.Functions
{
    public class PricesFunction : BaseFunction
    {
        public PricesFunction(ILogger<PricesFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("Prices")]
        [OpenApiOperation(operationId: "Prices", tags: new[] { "Prices" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<StockPrice>), Description = "The OK response")]
        public async Task<IActionResult> GetPrices(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/prices")] HttpRequestMessage req,
          string network, string coin,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).PriceProvider.GetPrices(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("Price")]
        [OpenApiOperation(operationId: "Price", tags: new[] { "Prices" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "currency", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StockPrice), Description = "The OK response")]
        public async Task<IActionResult> GetPrice(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/price/{token}/{currency}")] HttpRequestMessage req,
          string network, string coin, string token, string currency,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).PriceProvider.GetPrice(network, token, currency);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("Feed")]
        [OpenApiOperation(operationId: "PriceFeed", tags: new[] { "Prices" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "currency", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StockPrice), Description = "The OK response")]
        public async Task<IActionResult> GetPriceFeed(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/price/{token}/{currency}/feed")] HttpRequestMessage req,
        string network, string coin, string token, string currency,
        ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).PriceProvider.GetFeed(network, token, currency);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("Oracles")]
        [OpenApiOperation(operationId: "Oracles", tags: new[] { "Prices" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "currency", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(StockPrice), Description = "The OK response")]
        public async Task<IActionResult> GetOracles(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/price/{token}/{currency}/oracles")] HttpRequestMessage req,
       string network, string coin, string token, string currency,
       ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).PriceProvider.GetOracles(network, token, currency);

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
