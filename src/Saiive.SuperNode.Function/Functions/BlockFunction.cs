using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class BlockFunctions : BaseFunction
    {
        public BlockFunctions(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }


        [FunctionName("GetBlockByHash")]
        [OpenApiOperation(operationId: "GetBlockByHash", tags: new[] { "Block" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "hashOrHeight", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BlockModel), Description = "The OK response")]
        public async Task<IActionResult> GetBlockByHash(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/block/{hashOrHeight}")] HttpRequestMessage req,
           string network, string coin, string hashOrHeight,
           ILogger log)
        {
            try
            {
                if(hashOrHeight == "tip")
                {
                    return await GetCurrentHeight(req, network, coin);
                }
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetBlockByHeightOrHash(network, hashOrHeight);
                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetCurrentHeight")]
        [OpenApiOperation(operationId: "GetCurrentHeight", tags: new[] { "Block" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BlockModel))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetCurrentHeight(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/block/tip")] HttpRequestMessage req,
               string network, string coin)
        {

            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentHeight(network);
                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetLatestBlocks")]
        [OpenApiOperation(operationId: "GetLatestBlocks", tags: new[] { "Block" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BlockModel))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetLatestBlocks(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/block")] HttpRequest req,
             string network, string coin)
        {

            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetLatestBlocks(network);
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

