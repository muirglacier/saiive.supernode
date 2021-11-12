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
    public class MasterNodeFunction : BaseFunction
    {

        public MasterNodeFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {

        }

        [FunctionName("ListMasternodes")]
        [OpenApiOperation(operationId: "ListMasternodes", tags: new[] { "MasterNodes" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Masternode>), Description = "The OK response")]
        public async Task<IActionResult> ListMasternodes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/masternodes/list")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).MasterNodeProvider.ListMasternodes(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("ListActiveMasternodes")]
        [OpenApiOperation(operationId: "ListActiveMasternodes", tags: new[] { "MasterNodes" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Masternode>), Description = "The OK response")]
        public async Task<IActionResult> ListActiveMasternodes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/masternodes/list/active")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).MasterNodeProvider.ListActiveMasternodes(network);

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
