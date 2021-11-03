using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Export;
using Saiive.SuperNode.Model.Requests;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Function.Functions
{
    public class ExportFunction : BaseFunction
    {
        private readonly IExportHandler _exportHandler;
        private readonly string _serviceBusConnection;
        private readonly string _exportQueue;

        public ExportFunction(ILogger<BaseFunction> logger, ChainProviderCollection chainProviderCollection, IServiceProvider serviceProvider, IExportHandler exportHandler, IConfiguration config) : base(logger, chainProviderCollection, serviceProvider)
        {
            _exportHandler = exportHandler;

            _serviceBusConnection = config.GetConnectionString("ServiceBusConnection") ?? config["ServiceBusConnection"];
            _exportQueue = config["EXPORT_QUEUE"];
        }

        public async Task SendExportRequestToQ(ExportDto export, int timeout)
        {
            var topicClient = new TopicClient(_serviceBusConnection, _exportQueue, RetryPolicy.Default);
            await topicClient.SendAsync(new Message
            {
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(export)),
                CorrelationId = export.PaymentTxId,
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddSeconds(timeout)
            });
        }

        [FunctionName("ExportHandler")]
        public async Task ExportHandler(
            [ServiceBusTrigger("%EXPORT_QUEUE%", Connection = "ServiceBusConnection")]
            Message queueMessage,
           ILogger log)
        {
            try
            {
                var content = Encoding.UTF8.GetString(queueMessage.Body);
                var dto = JsonConvert.DeserializeObject<ExportDto>(content);

                await _exportHandler.Export(dto.Chain, dto.Network, dto.Addresses, dto.From, dto.To, dto.PaymentTxId, dto.Mail, dto.ExportType);
            }
            catch(Exception ex)
            {
                log.LogError(ex, "error whily creating export...");
            }
           
        }

        [FunctionName("EnqueueExport")]
        [OpenApiOperation(operationId: "EnqueueExport", tags: new[] { "Export" })]
        [OpenApiParameter(name: "network", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "coin", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExportDto), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
        public async Task<IActionResult> GetTotalBalance(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/{network}/{coin}/export/enqueu")] ExportDto req,
           string coin, string network)
        {
            try
            {
                await SendExportRequestToQ(req, 5000);

                return new NoContentResult();
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return new BadRequestObjectResult(new ErrorModel(e.Message));
            }
        }
    }
}
