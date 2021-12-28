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
using Saiive.SuperNode.Function.Base;

namespace Saiive.SuperNode.Function.Functions
{
    public class HealthCheckFunction : BaseFunction
    {
        private readonly Dictionary<string, double> _blockchainTimeCheckMinuteInterval;
        private const double DefaultCheckMinuteInterval = 300;

        public HealthCheckFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
            _blockchainTimeCheckMinuteInterval = new Dictionary<string, double>();
            _blockchainTimeCheckMinuteInterval.Add("BTC", DefaultCheckMinuteInterval);
            _blockchainTimeCheckMinuteInterval.Add("DFI", TimeSpan.FromMinutes(60).TotalMinutes);
        }

        [FunctionName("Health")]
        [OpenApiOperation(operationId: "Health", tags: new[] { "Health" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BlockModel), Description = "The OK response")]
        public async Task<IActionResult> Health(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/health")] HttpRequestMessage req,
          ILogger log)
        {
            await Task.CompletedTask;
            return new NoContentResult();
        }

        [FunctionName("HealthCheckNetwork")]
        [OpenApiOperation(operationId: "HealthCheckNetwork", tags: new[] { "Health" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BlockModel), Description = "The OK response")]
        public async Task<IActionResult> HealthCheckNetwork(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/health")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            try
            {
                var obj = await ChainProviderCollection.GetInstance(coin).BlockProvider.GetCurrentHeight(network);

                if (obj == null)
                {
                    throw new ArgumentException("block model is empty");
                }
                var time = Convert.ToDateTime(obj.Time);

                var checkInterval = DefaultCheckMinuteInterval;
                if (_blockchainTimeCheckMinuteInterval.ContainsKey(coin))
                {
                    checkInterval = _blockchainTimeCheckMinuteInterval[coin];
                }

                var timeStartCheck = DateTime.Now.AddMinutes(checkInterval * -1);
                var timeEndCheck = DateTime.Now.AddMinutes(checkInterval);

                if (time >= timeStartCheck && time <= timeEndCheck)
                {
                    return new OkObjectResult(obj);
                }

                return new BadRequestObjectResult("Chain is not synced yet!");
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}", e);
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
    }

}
