using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Controllers
{
    [ApiController]
    [Route("/api/v1/")]
    public class IcxController : BaseLegacyController
    {
        public IcxController(ILogger<GovController> logger, IConfiguration config) : base(logger, config)
        {

        }

        [HttpGet("{network}/{coin}/icx/orders")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetOrders(string coin, string network)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/icx/orders");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                return Ok(data);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/icx/orders/{orderTx}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetOrders(string coin, string network, string orderTx)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/icx/order/{orderTx}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                return Ok(data);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }


        [HttpGet("{network}/{coin}/icx/htlc/{offerTx}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorModel))]
        public async Task<IActionResult> GetHtlcOffer(string coin, string network, string offerTx)
        {
            var response = await _client.GetAsync($"{ApiUrl}/api/{coin}/{network}/icx/htlc/{offerTx}");

            try
            {
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();

                return Ok(data);
            }
            catch (Exception e)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new ErrorModel($"could not be found"));
                }
                Logger.LogError($"{e}");
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
