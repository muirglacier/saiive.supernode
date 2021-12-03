using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Saiive.Dobby.Api;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Function.Base;
using Saiive.SuperNode.Push.Model;

namespace Saiive.SuperNode.Push.Functions
{
    public class PushRegistrationFunction : BaseFunction
    {
        public PushRegistrationFunction(ILogger<PushRegistrationFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider, IDobbyService dobbyService) : base(logger, chainProviderCollection, serviceProvider)
        {
            DobbyService = dobbyService;
        }

        public IDobbyService DobbyService { get; }

        [FunctionName("PushRegistrationFunction")]
        [OpenApiOperation(operationId: "PushRegistrationFunction", tags: new[] { "Push" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushNotificationModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushNotificationModel), Description = "The OK response")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/push/register")] PushNotificationModel req,
              string network, string coin,

              [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<PushNotificationModel> pushModelCollector,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{PushToken}", PartitionKey = PushNotificationModel.PushTokenPartitionKey)]
            Document pushModelDoc,


            ILogger log)
        {
            if (coin.ToUpperInvariant() != "DFI")
            {
                return new NoContentResult();
            }

            if (pushModelDoc != null)
            {
                PushNotificationModel pushModel = (dynamic)pushModelDoc;

                foreach (var vaultId in req.VaultIds)
                {
                    try
                    {
                        var vault = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanVault(network, vaultId);

                        var addVaultResponse = await DobbyService.AddVaultForUser(vault.VaultId);
                        var addInfoTrigger = await DobbyService.CreateNotificationTrigger(vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate) * 2, "info");
                        var addwarningTrigger = await DobbyService.CreateNotificationTrigger(vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate) + 50, "warning");
                        var addErrorTrigger = await DobbyService.CreateNotificationTrigger(vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate) - 50, "error");



                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                }
            }


            return new OkObjectResult(null);
        }
        [FunctionName("PushDeregistrationFunction")]
        [OpenApiOperation(operationId: "PushDeregistrationFunction", tags: new[] { "Push" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushNotificationModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushNotificationModel), Description = "The OK response")]
        public async Task<IActionResult> Deregister(
                  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/push/deregister")] PushNotificationModel req,
                    string network, string coin,

                    [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<PushNotificationModel> pushModelCollector,

                  [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{PushToken}", PartitionKey = PushNotificationModel.PushTokenPartitionKey)]
            Document pushModelDoc,


                  ILogger log)
        {
            return new OkObjectResult(null);
        }
    }
}

