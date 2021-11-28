using System;
using System.Collections.Generic;
using System.Linq;
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
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Function.Functions
{
    public class AddressTransactionDetailFunction : BaseFunction
    {
        public AddressTransactionDetailFunction(ILogger<AddressTransactionDetailFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("AddressTransactionDetailFunctionGetTransactions")]
        [OpenApiOperation(operationId: "AddressTransactionDetailFunctionGetTransactions", tags: new[] { "AddressTransactionDetail" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Example = typeof(List<BlockTransactionModel>), Description = "The OK response")]
        public async Task<IActionResult> GetTransactions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{network}/{coin}/tx-history/{address}")] HttpRequestMessage req,
            string coin, string network, string address,
            ILogger log)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).AddressTransactionDetailProvider.GetTransactions(network, address);
                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("AddressTransactionDetailFunctionGetMultiTransactions")]
        [OpenApiOperation(operationId: "AddressTransactionDetailFunctionGetMultiTransactions", tags: new[] { "AddressTransactionDetail" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Example = typeof(List<BlockTransactionModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetCurrentHeight(
               [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/tx-history")] AddressesBodyRequest req,
               string coin, string network)
        {

            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var obj = await ChainProviderCollection.GetInstance(coin).AddressTransactionDetailProvider.GetTransactions(network, req);
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

