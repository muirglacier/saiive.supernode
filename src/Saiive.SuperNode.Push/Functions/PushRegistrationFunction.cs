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
using Saiive.Dobby.Api.Model;
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
        [OpenApiParameter(name: "language", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushNotificationModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/push/register/{language}")] PushNotificationModelRegister req,
              string network, string coin, string language,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<PushNotificationModel> pushModelCollector,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{PushToken}", PartitionKey = PushNotificationModelDatabase.PushTokenPartitionKey)]
            Document pushModelDoc,


            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<VaultIdPushTokenMapping> vaultMappingCollector,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{VaultId}", PartitionKey = VaultIdPushTokenMapping.VaultMapPartitionKey)]
            Document vaultMapDoc,

            ILogger log)
        {
            if (coin.ToUpperInvariant() != "DFI")
            {
                return new NoContentResult();
            }

            PushNotificationModelDatabase dbEntry = null;
            GetUserResponse dobbyUser = null;

            if (pushModelDoc != null)
            {
                dbEntry = (dynamic)pushModelDoc;
                dobbyUser = dbEntry.DobbyUser;
            }
            else
            {
                if (language == null || (language != "de" && language != "en"))
                {
                    throw new ArgumentException($"{nameof(language)} must be 'de' or 'en'.");
                }

                dbEntry = new PushNotificationModelDatabase
                {
                    PushToken = req.PushToken
                };
                dobbyUser = await DobbyService.SetupUser(language);
                dbEntry.DobbyUser = dobbyUser;
            }

            try
            {
                if (!dbEntry.VaultIds.Contains(req.VaultId))
                {


                    var vault = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanVault(network, req.VaultId);

                    try
                    {
                        var addVaultResponse = await DobbyService.AddVaultForUser(dobbyUser.UserId, vault.VaultId);
                    }
                    catch
                    {
                    }
                    var addInfoTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate) * 2, "info");
                    var addwarningTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate) + 50, "warning");
                    var addDailyTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate), "daily");
                    var addinLiqTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate), "inLiquidation");
                    var addMayLiqTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate), "mayLiquidate");
                    var addLiquidatedTrigger = await DobbyService.CreateNotificationTrigger(dobbyUser.UserId, vault.VaultId, Convert.ToInt32(vault.LoanScheme.InterestRate), "liquidated");


                    dbEntry.VaultIds.Add(req.VaultId);

                }
            }
            catch (Exception ex)
            {
                //ignore
            }

            VaultIdPushTokenMapping map;

            if(vaultMapDoc != null)
            {
                map = (dynamic)vaultMapDoc;
            }
            else
            {
                map = new VaultIdPushTokenMapping
                {
                    VaultId = req.VaultId
                };
            }

            map.PushTokenList.Add(req.PushToken);


            await pushModelCollector.AddAsync(dbEntry);
            await vaultMappingCollector.AddAsync(map);

            return new NoContentResult();
        }

        [FunctionName("PushDeregistrationFunction")]
        [OpenApiOperation(operationId: "PushDeregistrationFunction", tags: new[] { "Push" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushNotificationModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
        public async Task<IActionResult> Deregister(
                  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/push/deregister")] PushNotificationModelRegister req,
                    string network, string coin,


            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<PushNotificationModel> pushModelCollector,


            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{PushToken}", PartitionKey = PushNotificationModelDatabase.PushTokenPartitionKey)]
            Document pushModelDoc,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString")]
            IAsyncCollector<VaultIdPushTokenMapping> vaultMappingCollector,

            [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{VaultId}", PartitionKey = VaultIdPushTokenMapping.VaultMapPartitionKey)]
            Document vaultMapDoc,

                  ILogger log)
        {
            if (coin.ToUpperInvariant() != "DFI")
            {
                return new NoContentResult();
            }
            if(pushModelDoc == null)
            {
                return new NoContentResult();
            }
            PushNotificationModelDatabase dbEntry = (dynamic)pushModelDoc;

            try
            {

                var vault = await ChainProviderCollection.GetInstance(coin).LoanProvider.GetLoanVault(network, req.VaultId);
                var addVaultResponse = await DobbyService.DeleteVaultForUser(dbEntry.DobbyUser.UserId, vault.VaultId);
            }
            catch (Exception ex)
            {

            }

            if(vaultMapDoc != null)
            {
                VaultIdPushTokenMapping map = (dynamic)vaultMapDoc;
                map.PushTokenList.Remove(req.PushToken);

                await vaultMappingCollector.AddAsync(map);
            }
            
            await pushModelCollector.AddAsync(dbEntry);

            return new NoContentResult();
        }

        [FunctionName("GetRegisteredVaultIds")]
        [OpenApiOperation(operationId: "GetRegisteredVaultIds", tags: new[] { "Push" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "pushToken", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushNotificationModel), Description = "The OK response")]
        public async Task<IActionResult> GetRegisteredVaultIds(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/push/{pushToken}/list")] HttpWebRequest req,
                  string network, string coin, string pushToken,

                [CosmosDB("%CosmosDBName%", "%CosmosDBCollection%", ConnectionStringSetting = "CosmosConnectionString",
            Id = "{pushToken}", PartitionKey = PushNotificationModelDatabase.PushTokenPartitionKey)]
            Document pushModelDoc,


                ILogger log)
        {
            if (coin.ToUpperInvariant() != "DFI")
            {
                return new NoContentResult();
            }

            await Task.CompletedTask;

            PushNotificationModel dbEntry = (dynamic)pushModelDoc;
            return new OkObjectResult(dbEntry);
        }
    }
}

