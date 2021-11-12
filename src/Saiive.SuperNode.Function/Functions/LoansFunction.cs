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
    public class LoansFunction : BaseFunction
    {
        public LoansFunction(ILogger<LoansFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("GetLoanSchemes")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetSchemes(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/schemes")] HttpRequestMessage req,
          string network, string coin,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanSchemes(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetLoanCollaterals")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetCollaterals(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/collaterals")] HttpRequestMessage req,
          string network, string coin,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanCollaterals(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetLoanTokens")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetLoanTokens(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/tokens")] HttpRequestMessage req,
          string network, string coin,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanTokens(network);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }



        [FunctionName("GetLoanScheme")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetScheme(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/schemes/{id}")] HttpRequestMessage req,
          string network, string coin, string id,
          ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanScheme(network, id);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetLoanCollateral")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetCollateral(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/collaterals/{id}")] HttpRequestMessage req,
        string network, string coin, string id,
        ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanCollateral(network, id);

                return new OkObjectResult(obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetLoanToken")]
        [OpenApiOperation(operationId: "Schemes", tags: new[] { "Loans" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanScheme>), Description = "The OK response")]
        public async Task<IActionResult> GetToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/loans/tokens/{id}")] HttpRequestMessage req,
        string network, string coin, string id,
        ILogger log)
        {

            try
            {

                var obj = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanToken(network, id);

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
