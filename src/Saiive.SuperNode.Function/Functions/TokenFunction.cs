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
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class TokenFunction : BaseFunction
    {
        public TokenFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("ListTokens")]
        [OpenApiOperation(operationId: "ListTokens", tags: new[] { "Token" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<int, TokenModel>), Description = "The OK response")]
        public async Task<IActionResult> ListTokens(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/tokens")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).TokenProvider.GetAll(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("GetToken")]
        [OpenApiOperation(operationId: "GetToken", tags: new[] { "Token" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Masternode>), Description = "The OK response")]
        public async Task<IActionResult> GetToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/tokens/{token}")] HttpRequestMessage req,
            string network, string coin, string token,
            ILogger log)
        {
            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).TokenProvider.GetToken(network, token);

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

