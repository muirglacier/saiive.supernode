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
using Saiive.SuperNode.Function.Base;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Requests;

namespace Saiive.SuperNode.Function.Functions
{
    public class AddressFunctions : BaseFunction
    {
        public AddressFunctions(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("GetTotalBalanceSingle")]
        [OpenApiOperation(operationId: "GetTotalBalance", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<AccountModel>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/balance-all/{address}")] HttpRequestMessage req,
            string network, string coin, string address,
            ILogger log)
        {
            try
            {
                var balanceTokens = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTotalBalance(network, address);

                return new OkObjectResult(balanceTokens);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetTotalBalanceMulti")]
        [OpenApiOperation(operationId: "GetTotalBalanceMulti", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<string, AccountModel>), Description = "The OK response")]
        public async Task<IActionResult> GetTotalBalance(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/balance-all")] AddressesBodyRequest req,
            string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var retAccountList = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTotalBalance(network, req);

                return new OkObjectResult(retAccountList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("BalanceSingle")]
        [OpenApiOperation(operationId: "BalanceSingle", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BalanceModel))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetBalance(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/balance/{address}")] HttpRequest req,
               string coin, string network, string address)
        {

            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetBalance(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("BalanceMulti")]
        [OpenApiOperation(operationId: "BalanceMulti", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<BalanceModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetBalances(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/balance")] AddressesBodyRequest req,
              string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetBalance(network, req);

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("AccountSingle")]
        [OpenApiOperation(operationId: "AccountSingle", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<AccountModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetAccount(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/account/{address}")] HttpRequest req,
            string coin, string network, string address)
        {

            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAccount(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("AccountsMulti")]
        [OpenApiOperation(operationId: "AccountMulti", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Account>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetAccounts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/accounts")] AddressesBodyRequest req,
            string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var retList = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAccount(network, req);

                return new OkObjectResult(retList);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("TransactionSingle")]
        [OpenApiOperation(operationId: "TransactionSingle", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType:typeof(object), Example = typeof(List<TransactionModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetTransactions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/txs/{address}")] HttpRequest req,
            string coin, string network, string address)
        {
            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTransactions(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("TransactionMulti")]
        [OpenApiOperation(operationId: "TransactionMulti", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Example = typeof(List<TransactionModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiTransactions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/txs")] AddressesBodyRequest req,
            string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetTransactions(network, req);
                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("TransactionUnspentSingle")]
        [OpenApiOperation(operationId: "TransactionUnspentSingle", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Example = typeof(List<TransactionModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetUnspentTransactionOutput(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/unspent/{address}")] HttpRequest req,
            string coin, string network, string address)
        {
            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetUnspentTransactionOutput(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("TransactionUnspentMulti")]
        [OpenApiOperation(operationId: "TransactionUnspentMulti", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Example = typeof(List<TransactionModel>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetMultiUnspentTransactionOutput(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/unspent")] AddressesBodyRequest req,
              string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetUnspentTransactionOutput(network, req);
                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }


        [FunctionName("LoanVaultsForAddress")]
        [OpenApiOperation(operationId: "LoanVaultsForAddress", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanVault>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> LoanVaultsForAddress(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/address/loans/vaults/{address}")] HttpRequestMessage req,
               string coin, string network, string address)
        {

            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetLoanVaultsForAddress(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("LoanVaultsForAddresses")]
        [OpenApiOperation(operationId: "LoanVaultsForAddresses", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanVault>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> LoanVaultsForAddresses(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/address/loans/vaults")] AddressesBodyRequest req,
              string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetLoanVaultsForAddresses(network, req.Addresses);

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
        [FunctionName("GetAuctionHistoryForAddress")]
        [OpenApiOperation(operationId: "GetAuctionHistoryForAddress", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "address", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanVault>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetAuctionHistoryForAddress(
               [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/address/loans/auctionhistory/{address}")] HttpRequestMessage req,
               string coin, string network, string address)
        {

            try
            {
                return new OkObjectResult(await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAuctionHistory(network, address));
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

        [FunctionName("GetAuctionHistoryForAddresses")]
        [OpenApiOperation(operationId: "GetAuctionHistoryForAddresses", tags: new[] { "Address" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddressesBodyRequest), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LoanVault>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ErrorModel))]
        public async Task<IActionResult> GetAuctionHistoryForAddresses(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/address/loans/auctionhistory")] AddressesBodyRequest req,
              string coin, string network)
        {
            try
            {
                req.Addresses = req.Addresses.Distinct().ToList();
                var ret = await ChainProviderCollection.GetInstance(coin).AddressProvider.GetAuctionHistory(network, req);

                return new OkObjectResult(ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
    }
}

