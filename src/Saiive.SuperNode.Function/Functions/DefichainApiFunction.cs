using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Function.Functions
{
    public class DefichainApiFunction : BaseFunction
    {
        private YieldFramingModelRequest _lastValidItem = null;
        public DefichainApiFunction(ILogger<AddressFunctions> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider) : base(logger, chainProviderCollection, serviceProvider)
        {
        }

        [FunctionName("ListYieldFarming")]
        [OpenApiOperation(operationId: "CoinPrice", tags: new[] { "DeFiChain" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<YieldFramingModel>), Description = "The OK response")]
        public async Task<IActionResult> ListYieldFarming(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/{network}/{coin}/list-yield-farming")] HttpRequestMessage req,
            string network, string coin,
            ILogger log)
        {
            var response = await _client.GetAsync($"https://api.defichain.io/v1/listyieldfarming?network={network}");

            try
            {
                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                YieldFramingModelRequest obj = JsonConvert.DeserializeObject<YieldFramingModelRequest>(data);

                _lastValidItem = obj ?? throw new ArgumentException();

                return new OkObjectResult(_lastValidItem.Pools);
            }
            catch (Exception e)
            {
                if (_lastValidItem != null)
                {
                    return new OkObjectResult(_lastValidItem.Pools);
                }

                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }

       
    }
}

