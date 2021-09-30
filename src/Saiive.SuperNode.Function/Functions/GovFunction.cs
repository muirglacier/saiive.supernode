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
    public class GovFunction : BaseFunction
    {
        private readonly Dictionary<string, double> _blockchainTimeCheckMinuteInterval;
        private const double DefaultCheckMinuteInterval = 300;

        public GovFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
            _blockchainTimeCheckMinuteInterval = new Dictionary<string, double>();
            _blockchainTimeCheckMinuteInterval.Add("BTC", DefaultCheckMinuteInterval);
            _blockchainTimeCheckMinuteInterval.Add("DFI", TimeSpan.FromMinutes(60).TotalMinutes);
        }

        [FunctionName("Gov")]
        [OpenApiOperation(operationId: "Gov", tags: new[] { "Gov" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<string, object>), Description = "The OK response")]
        public async Task<IActionResult> GetGov(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/gov")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            await Task.CompletedTask;
            try
            {
                var dic = new Dictionary<string, object>();
                dic.Add("LP_DAILY_DFI_REWARD", 255405.071232);
                return new OkObjectResult(dic);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}", e);
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
    }

}
