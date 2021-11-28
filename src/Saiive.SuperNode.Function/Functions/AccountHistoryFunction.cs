using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Saiive.SuperNode.Function.Functions
{
    public class AccountHistoryFunction : BaseFunction
    {
        public AccountHistoryFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("GetAccountHistory")]
        [OpenApiOperation(operationId: "GetAccountHistory", tags: new[] { "AccountHistory" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Path, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "maxBlockHeight", In = ParameterLocation.Path, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "no_rewards", In = ParameterLocation.Path, Required = false, Type = typeof(bool))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<AccountHistory>), Description = "The OK response")]
        public async Task<IActionResult> GetAccountHistory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/accounthistory/{address}/{token}/{limit}/{maxBlockHeight}/{no_rewards}")] HttpRequestMessage req,
            string coin, string network, string address, string token, string limit, string maxBlockHeight, bool no_rewards,
            ILogger log)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).AccountHistoryProvider.GetAccountHistory(network, address, token, limit, maxBlockHeight, no_rewards);
                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetTotalBalance")]
        [OpenApiOperation(operationId: "GetTotalBalance", tags: new[] { "Block" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "token", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Path, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "maxBlockHeight", In = ParameterLocation.Path, Required = false, Type = typeof(string))]
        [OpenApiParameter(name: "no_rewards", In = ParameterLocation.Path, Required = false, Type = typeof(bool))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<AccountHistory>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetCurrentHeight(
               [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/history-all/{token}/{limit}/{maxBlockHeight}/{no_rewards}")] AddressesBodyRequest req,
               string coin, string network, string token, string limit, string maxBlockHeight, bool no_rewards)
        {

            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var obj = await ChainProviderCollection.GetInstance(coin).AccountHistoryProvider.GetTotalBalance(network, token, limit, maxBlockHeight, no_rewards, req);
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

