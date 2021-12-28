using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    public class SendToAddressBody
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }
    }

    [ApiController]
    [Route("/api/v1/")]
    public class WalletController : BaseController
    {
        public WalletController(ILogger<WalletController> logger, IConfiguration config) : base(logger, config)
        {
        }

        [HttpPost("{network}/{coin}/sendtoaddress")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        public async Task<IActionResult> SendToAddress(string coin, string network, [FromBody] SendToAddressBody body)
        {
            try
            {
                var enableSendConfig = Config.GetSection("ENABLE_SEND");
                var maxSendAmountConfig = Config.GetSection("MAX_SEND_AMOUNT");

                var isEnabled = (bool)enableSendConfig.GetValue(typeof(bool), $"{coin}_{network}", false);

                if (!isEnabled)
                {
                    return BadRequest($"Send is not enabled for {network}");
                }

                var maxSendAmount = (double)maxSendAmountConfig.GetValue(typeof(double), $"{coin}_{network}", 1);

                if (body.Amount > maxSendAmount)
                {
                    return BadRequest($"Max amount to send is {maxSendAmount}!");
                }


                var httpContent =
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{ApiUrl}/api/{coin}/{network}/wallet/rpc/sendtoaddress", httpContent);

                var data = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                return Ok(data);
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
